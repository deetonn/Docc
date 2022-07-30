using System.Net;
using System.Net.Sockets;

using Newtonsoft.Json;

using Docc.Common.Auth;
using Docc.Common.Storage;
using Docc.Common;

using Docc.Server.Server;
using Docc.Server.Data;
using Docc.Server.Server.Auth;

namespace Docc.Server;

internal enum ServerType
{
    Local,
    // todo
}

internal class ServerConnection
{
    public readonly ILogger Logger;
    private IAuthorizationService _authService;

    // TODO:
    // when a user connects, make them supply a username + password.
    // the password should be hashed as soon as they have entered it
    // and sent to the server via 'IStorageItem'.
    // Assign them a unique identifier (sort of sessionId)
    // and append them into _storage
    private IStorageContainer? _storage;

    public void AddUser(string name, string pw)
    {
        _storage?.Add(new(name, pw));
        Task.Run(() => _storage?.Save());
    }

    public void UseAuthorization<T>() where T : IAuthorizationService, new()
    {
        _authService = new T();
    }
    public void InitStorage(string path)
    {
        _storage = new StorageContainer(path);
    }

    public string Version { get; }
    public string AppName { get; }

    protected Thread Listener { get; }
    protected Thread MessageAcceptor { get; }
    protected bool Running { get; } = true;

    public IPAddress Address { get; }
    public IPEndPoint Endpoint { get; }                                     // for now
    public IPHostEntry Host { get; set; } = Dns.GetHostEntry("localhost");

    private ServerContext Context { get; }

    public ServerContext ContextView()
    {
        return Context;
    }

    public ServerConnection(ServerType connType)
    {
        InitStorage("saved.json");

        Logger = new ServerConsoleLogger();
        _authService = new PrivateServerAuthorization();

        // start the server on ServerSocket.

        Version = Environment.GetEnvironmentVariable("App-Version") ?? "(no version)";
        AppName = Environment.GetEnvironmentVariable("App-Agent") ?? "Docc";

        // TODO:
        /*
         * Connections needs to be more in-depth.
         * Maybe it's own object, that contains Client-Socket keypairs,
         * but also matches ServerClients to sessionIds.
         */

        Logger.Log($"starting server. ({AppName})");

        if (connType != ServerType.Local)
        {
            throw new NotImplementedException($"ServerType.{connType} is not implemented");
        }

        Context = ServerContext.Local(25755);

        Listener = new(() =>
        {
            while (Running)
            {
                // Socket
                var newConnection = Context.Socket.Accept();

                // could block this thread..
                var firstPacket = newConnection.ReceiveEncrypted();

                if (firstPacket is null)
                {
                    // failed to deserialize initial packet,
                    // fuck off!

                    var rejection = new RequestBuilder()
                        .WithResult(RequestResult.BadPacket)
                        .Build();

                    newConnection.SendRequest(rejection);
                    newConnection.Dispose();
                    continue;
                }

                if (firstPacket == Request.Timeout)
                {
                    // failed to accept first request
                    // fuck off!

                    var rb = new RequestBuilder()
                        .WithResult(RequestResult.Disconnecting)
                        .AddContent("Failed to respond to initial packet.")
                        .Build();

                    newConnection.SendRequest(rb);
                    newConnection.Dispose();
                    continue;
                }

                if ((!firstPacket.Arguments.ContainsKey("userName"))
                    || (!firstPacket.Arguments.ContainsKey("hashedPw")))
                {
                    // bad request
                    newConnection.SendRequest(
                        new RequestBuilder()
                            .WithResult(RequestResult.BadCredentials)
                            .Build()
                    );
                    newConnection?.Dispose();
                    continue;
                }

                var userName = firstPacket.Arguments["userName"];
                var hashedPw = firstPacket.Arguments["hashedPw"];

                if (_storage?.Contains(new StorageItem(userName, hashedPw)) != 0)
                {
                    newConnection.SendRequest(
                        new RequestBuilder()
                            .AddContent("That username or password is not recognized.")
                            .WithResult(RequestResult.BadCredentials)
                            .Build()
                    );

                    newConnection?.Dispose();
                    continue;
                }

                // at this point we know who we're talking to

                var connection = new Connection
                {
                    Client = _storage.Get(userName),
                    Socket = newConnection,
                    SessionKey = new()
                };

                if (!OnConnection(connection))
                {
                    newConnection.SendRequest(
                        new RequestBuilder()
                        .WithResult(RequestResult.NotAuthorized)
                        .Build()
                    );
                    connection.Dispose();
                    continue;
                }

                // We want to send the client their sessionId.
                // So that any future requests can verify them that way.

                Logger.Log($"accepted client `{connection?.Client?.Name}`");

                Context.Connections.Add(connection!);

                var sessionIdRequest = new RequestBuilder()
                    .WithResult(RequestResult.OK)
                    .WithArguments(new() { { "session_id", connection!.SessionKey.Value.ToString() } });

                // This tells the client they're connected.
                connection.Socket.SendRequest(sessionIdRequest.Build());
            }
        });
        MessageAcceptor = new(() =>
        {
            while (Running)
            {
                SpinWait.SpinUntil(() => Context.Connections.Any(x => x.Socket.Available > 0));
                var clientsWhoSentMessages = Context.Connections.Where(x => x.Socket.Available > 0);

                List<(Request, Connection)> sentMessages = new();

                foreach (var client in clientsWhoSentMessages)
                {
                    if (client.Socket is null)
                    {
                        // why the fuck is it null
                        continue;
                    }

                    var message = client.Socket?.ReceiveEncrypted()
                        ?? Request.Default;

                    // In this case, the client hasn't even supplied
                    // a session key argument.
                    if (!message.Arguments.ContainsKey("session_id"))
                    {
                        // deny the request.

                        var badCreds = new RequestBuilder()
                            .WithResult(RequestResult.BadCredentials)
                            .AddContent("client didn't supply session id.")
                            .Build();

                        client?.Socket?.SendRequest(badCreds);

                        return;
                    }

                    var suppliedSessionKey = message.Arguments["session_id"];

                    // In this case, they've supplied an invalid (or old key).
                    if (!Context.SessionUser(suppliedSessionKey, out Connection? connection))
                    {
                        // Maybe create temporary (not saved to disk) storage of
                        // each users past session ids. Then in this case, we can 
                        // check if they're a real client or not. If an old session key
                        // is provided, supply them with a new??

                        // ^^^ just an idea.

                        var badSession = new RequestBuilder()
                            .WithResult(RequestResult.ExpiredCredentials)
                            .AddContent("your session credentials have expired.")
                            .Build();

                        client.Socket?.SendRequest(badSession);
                        continue;
                    }

                    if (message?.Result == RequestResult.Disconnecting)
                    {
                        Logger?.Log($"{client.Client?.Name} has disconnected.");
                        Context.Connections.Remove(client);
                        continue;
                    }

                    // `client.Socket` will not be null, as it's checked above.
                    // the `throw null!` is there to tell IDE's to stfu
                    sentMessages.Add((message!, connection!));
                }

                foreach (var sentMessage in sentMessages)
                {
                    OnMessage(sentMessage.Item1, sentMessage.Item2);
                }
            }
        });

        Listener.Start();
        MessageAcceptor.Start();
    }

    // TODO:
    /*
     * Make the delegate also take in the server context. I.E: All users
     * and a sessionId so it can identify who is making the request.
     */
    public Action<Request, Connection> OnMessage { get; set; }
        = delegate (Request request, Connection conn)
        {
            conn.Socket?.SendRequest(
                new RequestBuilder()
                    .WithResult(RequestResult.GenericError)
                    .AddContent("You have not set Server.OnMessage to a custom message handler!")
                    .Build());
        };

    public Func<Connection, bool> OnConnection { get; set; }
        = delegate (Connection conn)
        {
            return true;
        };
}
