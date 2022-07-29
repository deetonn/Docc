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
}
