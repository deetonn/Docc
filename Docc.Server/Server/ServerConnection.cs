﻿using System.Net;
using System.Net.Sockets;

using Newtonsoft.Json;

using Docc.Common.Auth;
using Docc.Common.Data;
using Docc.Common.Storage;
using Docc.Common;

using Docc.Server.Server;
using Docc.Server.Data;

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

    public Dictionary<ServerClient, Socket> Connections { get; }

    protected Thread Listener { get; }
    protected Thread MessageAcceptor { get; }
    protected Thread Validator { get; }
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
                var newConnection = Context.Socket.Accept();
            }
        });
        MessageAcceptor = new(() =>
        {
            while (Running)
            {
                SpinWait.SpinUntil(() => Context.Connections.Any(x => x.Socket.Available > 0));
                var clientsWhoSentMessages = Context.Connections.Where(x => x.Socket.Available > 0);

                List<(Request?, Socket)> sentMessages = new();

                foreach (var client in clientsWhoSentMessages)
                {
                    if (client.Socket is null)
                    {
                        // why the fuck is it null
                        continue;
                    }

                    var message = client.Socket?.ReceiveEncrypted()
                        ?? Request.Default;

                    if (message?.Result == RequestResult.Disconnecting)
                    {
                        Logger?.Log($"{client.Client?.Name} has disconnected.");
                        Context.Connections.Remove(client);
                        continue;
                    }

                    // `client.Socket` will not be null, as it's checked above.
                    // the `throw null!` is there to tell IDE's to stfu
                    sentMessages.Add((message, client.Socket ?? throw null!));
                }

                foreach (var sentMessage in sentMessages)
                {
                    OnMessage(sentMessage.Item1 ?? Request.Default, sentMessage.Item2);
                }
            }
        });
        Validator = new(() =>
        {
            //  TODO:

            /*
             * This currently doesn't work, 
             * when users disconnect, they aren't removed.
             * Once the overhaul of how we store users in memory is done,
             * I will fix this. - Deeton
             */
            while (true)
            {
                ValidateUsers();
                // not ideal, but does its job for the time being.
                Thread.Sleep(TimeSpan.FromSeconds(5));
            }
        });

        Listener.Start();
        MessageAcceptor.Start();
        Validator.Start();
    }

    // TODO:
    /*
     * Make the delegate also take in the server context. I.E: All users
     * and a sessionId so it can identify who is making the request.
     */
    public Action<Request, Socket> OnMessage { get; set; }
        = delegate (Request req, Socket client)
        {

        };

    public Func<Connection, bool> OnConnection { get; set; }
        = delegate (Connection conn)
        {
            return true;
        };

    public void EditUser(string nameOrId, Func<ServerClient, ServerClient> man)
    {
        if (Connections.Any(x => x.Key.Name == nameOrId || x.Key.Id.ToString() == nameOrId))
            return;

        var user = Connections.Where(x => x.Key.Name == nameOrId || x.Key.Id.ToString() == nameOrId).First();
        Connections.Remove(user.Key);
        Connections.Add(man(user.Key), user.Value);
    }

    // iterate all users & remove any who's connection
    // has dropped.
    private void ValidateUsers()
    {
        List<Socket> schedule = new();

        foreach (var (user, conn) in Connections)
        {
            // TODO:
            /*
             * 'conn.Connected' will not be set to false until we fail to make a request.
             * My idea is to automate this, so the client sends a packet before they exit
             * letting us know they've exited.
             */
            if (!conn.Connected)
            {
                schedule.Add(conn);
                Logger.Log($"client '{user.Name}' has disconnected.");
            }
        }

        schedule.ForEach(conn => conn.Close());
    }
}
