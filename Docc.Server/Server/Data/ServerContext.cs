using System.Net;
using System.Net.Sockets;
using System.Text;

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
    public bool EditUserById(string userId, Func<Connection, Connection> editor)
    {
        if (!TryGetUserById(userId, out Connection? conn))
        {
            return false;
        }

        var index = Connections.IndexOf(conn!);

        Connections[index] = editor(conn!);
        return true;
    }

#if DEBUG
    internal string DebugView()
    {
        StringBuilder view = new();
        var version = Environment.GetEnvironmentVariable("App-Version");
        var introduction = $"Docc [server] {version}";

        view.AppendLine($"Docc [server] {version} (debug view)");
        view.AppendLine(new string('^', introduction.Length));

        view.AppendLine();

        view.AppendLine($"address: {Address}");
        view.AppendLine($"endpoint: {Endpoint}\n");

        view.AppendLine(" == Socket Info ==\n");

        view.AppendLine($"available bytes: {Socket.Available}");
        view.AppendLine($"buffer size: {Socket.ReceiveBufferSize}");

        foreach (var conn in Connections)
        {
            view.AppendLine($"\n\n ==Connection Info for '{conn.Client!.Name}'==\n");
            view.AppendLine($"clientId: {conn.Client.UserId}");
            view.AppendLine($"permissions: {string.Join(", ", conn.Client.Permissions)}");
            view.AppendLine($"tags: {string.Join(", ", conn.Client.Tags)}\n");

            view.AppendLine($"sessionId: {conn.SessionKey.Value}");
            view.AppendLine($"expiresAt: {conn.SessionKey.ExpiresAt.ToLongDateString()} {conn.SessionKey.ExpiresAt.ToLongTimeString()}\n");

            view.AppendLine($"availableBytes : {conn.Socket!.Available}");
        }

        return view.ToString();
    }
#endif
}
