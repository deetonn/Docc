using System.Net;
using System.Net.Sockets;

namespace Docc.Server.Data;

internal class ServerContext
{
    public static ServerContext Local(int port)
    {
        IPHostEntry Host = Dns.GetHostEntry("localhost");
        var address = Host.AddressList[1];
        var socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        var endpoint = new IPEndPoint(address, port);
        socket.Bind(endpoint);
        socket.Listen(10);

        return new ServerContext
        {
            Address = address,
            Endpoint = endpoint,
            Socket = socket,
            Connections = new()
        };
    }

    public List<Connection> Connections { get; init; }

    public IPAddress Address { get; init; }
    public IPEndPoint Endpoint { get; init; }

    public Socket Socket { get; init; }

    public bool SessionUser(string sessionId, out Connection? conn)
    {
        if (!Connections.Where(x => x.SessionKey.Value.ToString() == sessionId).Any())
        {
            conn = null;
            return false;
        }

        conn = Connections.Where(x => x.SessionKey.Value.ToString() == sessionId).First();
        return true;
    }
    public bool SessionUser(Guid sessionId, out Connection? conn)
    {
        if (!Connections.Where(x => x.SessionKey.Value == sessionId).Any())
        {
            conn = null;
            return false;
        }

        conn = Connections.Where(x => x.SessionKey.Value == sessionId).First();
        return true;
    }

    public bool TryGetUserById(string userId, out Connection? conn)
    {
        var users = Connections.Where(x => x.Client?.UserId.ToString() == userId);

        if (!users.Any())
        {
            conn = null;
            return false;
        }

        conn = users.First();
        return true;
    }
}
