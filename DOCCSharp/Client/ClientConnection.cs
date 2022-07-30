using Docc.Common;
using Docc.Common.Data;
using Docc.Common.Storage;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;

namespace Docc.Client;

internal class ClientConnection
{
    private readonly ILogger _logger;

    protected Socket Socket { get; }

    public bool Connected { get; private set; } = false;
    public string SessionId { get; private set; } = string.Empty;

    public IPAddress Address { get; }
    public IPHostEntry Entry { get; } = Dns.GetHostEntry("localhost");
    public IPEndPoint ServerEndpoint { get; }

    public ClientConnection(string userName, string password)
    {
        _logger = new ClientConsoleLogger();

        // at this point, if the local user is banned, they are gone.

        Address = Entry.AddressList[1];
        ServerEndpoint = new IPEndPoint(Address, 25755);
        Socket = new Socket(Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        int attempts = 0;

        while (attempts <= 5)
        {
            try
            {
                _logger.Log($"attempting to connect to the server");
                Socket.Connect(ServerEndpoint);
            }
            catch
            {
                attempts++;
                continue;
            }

            break;
        }

        if (attempts == 5)
        {
            _logger.Log("failed to connect to the server.");
            Exit(0);
        }

        // We're connected, instantly send the client info over.

        _logger.Log($"resolved host [{ServerEndpoint.Address}:{ServerEndpoint.Port}, {ServerEndpoint.AddressFamily}]");

        var rb = new RequestBuilder()
            .WithArguments(new() { { "userName", userName }, { "hashedPw", StorageUtil.Sha256Hash(password) } });

        Socket.SendEncrypted(rb.Build(), _logger);
        var status = Socket.ReceiveEncrypted(_logger);

        if (status is null)
        {
            // tf?
            _logger.Log("server failed to respond with handshake.");
            Exit(0);
            // so IDE's can see that beyond this point status will not
            // be null.
            return;
        }

        if (status.Result != RequestResult.OK)
        {
            _logger.Log($"server rejected the connection with reason: {Translation.From(status.Result).Conversion}");
            Exit(-1);
        }

        _logger.Log("connected to server!");
        Connected = true;
        SessionId = status.Arguments["session_id"];
    }

    public Request? MakeRequest(Request query)
    {
        _logger.Log($"making request to '{query.Location}'");

        Socket.SendEncrypted(query, _logger);
        var res = Socket.ReceiveEncrypted();
        _logger.Log($"received response '{res?.Location}' ({res?.Result})");
        return res;
    }
}
