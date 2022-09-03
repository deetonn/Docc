using Docc.Common;
using Docc.Common.Data;
using Docc.Common.Storage;
using System.Net;
using System.Net.Sockets;

namespace Docc.Client;

// This will need to be mega modified.
// OR -- create a type that implements ILogger
// that interacts with a certain TextBox?

public class ClientConnection
{
    private readonly ILogger _logger;

    protected Socket Socket { get; }

    public bool Connected { get; private set; } = false;
    public string SessionId { get; private set; } = string.Empty;
    public string LastMessage { get; private set; } = string.Empty;

    private Thread MessageAccepter { get; set; }

    public IPAddress Address { get; }
    public IPHostEntry Entry { get; } = Dns.GetHostEntry("localhost");
    public IPEndPoint ServerEndpoint { get; }

    // raw flag specifies the password is already hashed ( for saving locally )

    public ClientConnection(string userName, string password, bool raw = false)
    {
        _logger = new ClientConsoleLogger();

        Address = Entry.AddressList[1];
        ServerEndpoint = new IPEndPoint(Address, 25755);
        Socket = new Socket(Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        int attempts = 0;

        while (attempts <= 5)
        {
            try
            {
                _logger.Log(this, $"attempting to connect to the server");
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
            _logger.Log(this, "failed to connect to the server.");
        }

        _logger.Log(this, $"resolved host [{ServerEndpoint.Address}:{ServerEndpoint.Port}, {ServerEndpoint.AddressFamily}]");

        var rb = new RequestBuilder()
            .WithArguments(new() { { "userName", userName }, { "hashedPw", raw ? password : StorageUtil.Sha256Hash(password) } });

        Socket.SendEncrypted(rb.Build(), _logger);
        var status = Socket.ReceiveEncrypted(_logger);

        if (status is null)
        {
            _logger.Log(this, "server failed to respond with handshake.");
            // so IDE's can see that beyond this point status will not
            // be null.
            return;
        }

        if (status.Result != RequestResult.OK)
        {
            _logger.Log(this, $"server rejected the connection with reason: {Translation.From(status.Result).Conversion}");
            Connected = false;

            if (status.Content.Any())
                LastMessage = status.Content.First();

            return;
        }

        _logger.Log(this, "connected to server!");
        Connected = true;
        SessionId = status.Arguments["session_id"];

        MessageAccepter = new(() =>
        {
            while (Connected)
            {
                SpinWait.SpinUntil(() => Socket.Available > 0);
                var content = Socket.ReceiveEncrypted(_logger);

                if (content is null)
                    continue;

                OnRequest(content);
            }
        });
        MessageAccepter.Start();
    }

    public Action<Request> OnRequest { get; set; }
        = delegate (Request req) { };

    public Request? MakeRequest(Request query)
    {
        _logger.Log(this, $"making request to '{query.Location}'");

        Socket.SendEncrypted(query, _logger);
        var res = Socket.ReceiveEncrypted();
        _logger.Log(this, $"received response '{res?.Location}' ({res?.Result})");
        return res;
    }

    public void MakeSpurnRequest(Request query)
    {
        Socket.SendEncrypted(query);
    }
}
