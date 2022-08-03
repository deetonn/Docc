using static Docc.Common.StaticHelpers;

using Docc.Common;
using Docc.Server;
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Docc.Server.Server;
using Docc.Server.Server.Auth;
using Docc.Common.Storage;
using Docc.Server.Endpoints;
using System.Drawing;
using Pastel;

Environment.SetEnvironmentVariable("App-Version", "v0.0.6-dev.1");
Environment.SetEnvironmentVariable("App-Agent", $"Docc {Environment.GetEnvironmentVariable("App-Version")}");

DirectoryListingManager manager = new();
ServerConnection connection = new(ServerType.Local);
CommandList context = new(new CommandLogger());

connection.UseLogger<ServerConsoleLogger>();

AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
{
    if (Environment.ExitCode == 0)
        return;

    // dump local data

    StringBuilder sb = new();

    sb.AppendLine("ServerCrashDump\n");
    sb.AppendLine($"EndpointCount: {manager.Count()}");
    sb.AppendLine($"AppName: {connection.AppName}");
    sb.AppendLine("\nErrorInfo:");

    var errorCode = Marshal.GetLastWin32Error();
    string errorMessage = new Win32Exception(errorCode).Message;

    sb.AppendLine(errorMessage + $" (0x{errorCode:X2})");

    File.WriteAllText("crash_dump.CD", sb.ToString());
};

// The 'Location' could be viewed as (in raw form)
// '/api/private/index?Key=Value'
// The .WithArguments method basically makes this
// usage pointless, but any raw request will look that
// way.
// 
// NOTE: Any request that is returning data, primarily
//       server functions, should return to location '/'.
//       This doesn't really matter a whole lot, but it makes
//       it more explicit that you're not meaning to send this
//       anywhere in particular.
//       It's simply a response, with a focus on the content.

// will generate a random number based on our spec
// and send it to the client.

connection.UseAuthorization<PrivateServerAuthorization>();
connection.UseLogger<ServerConsoleLogger>();

Random serverRng = new();
var version = Environment.GetEnvironmentVariable("App-Version")!;

/*
 * ENDPOINTS:
 * 
 * All endpoints the client can be served by are here
 */

manager.MapGet("/api/v1/ping", (args, conn) =>
{
    return new RequestBuilder()
        .WithResult(RequestResult.OK)
        .AddContent("Pong!")
        .Build();
});
manager.MapGet("/api/v1/disconnect", (args, conn) =>
{
    // simply terminate their session ID
    // don't really care about closing the socket.

    // to ensure the socket doesn't die before 
    // returning the Ok
    RunIn(5, () =>
    {
        conn.Disconnect();
    });

    return new RequestBuilder()
        .WithResult(RequestResult.OK)
        .Build();
});
manager.MapGet("/chat/api/sendMessage", (args, conn) =>
{
    if (!conn.Client.Permissions.Contains("send_message"))
    {
        var rejectionMessage = new RequestBuilder()
            .WithLocation("/chat/api/receive")
            .WithResult(RequestResult.OK)
            .WithArguments(new()
            {
                    { "name", "SERVER" },
                    { "color", "DarkRed" },
                    { "msg", "You do not have permission to speak right now." }
            });

        conn.Socket?.SendRequest(rejectionMessage.Build());
        return new RequestBuilder().WithResult(RequestResult.NotAuthorized).Build();
    }

    var error = new RequestBuilder()
        .WithLocation("/error/handle")
        .WithResult(RequestResult.BadArguments)
        .AddContent("request is missing required argument.")
        .Build();

    var color = Color.White;
    string name = conn.Client?.Name ?? "Retard";

    if (args.ContainsKey("name"))
        name = args["name"];
    if (!args.ContainsKey("msg"))
        return error;
    if (args.ContainsKey("col"))
    {
        try
        {
            color = Color.FromName(args["col"]);
        }
        catch (Exception)
        {
            // doesnt matter, color will be white.
        }
    }

    if (conn.Client?.Tags.Any() is true)
    {
        var beforeTags = name;
        name = string.Empty;

        foreach (var tag in conn.Client?.Tags ?? Array.Empty<string>().ToList())
        {
            name += $"[{tag}]";
        }

        name += $" {beforeTags}";
    }

    var message = args["msg"];

    // sanitise string...

    Task.Run(() =>
    {
        var request = new RequestBuilder()
            .WithLocation("/chat/api/receive")
            .WithResult(RequestResult.OK)
            .WithArguments(new()
                            {
                                { "name", name },
                                { "color", color.Name },
                                { "msg", message }
                            });

        foreach (var cl in connection.ContextView().Connections)
        {
            if (cl.Client?.Name == conn.Client?.Name)
            {
                continue;
            }

            // all accounts will contain this permission unless it's been
            // explicitly taken from them.

            cl?.Socket?.SendRequest(request.Build());
        }
    });

    return Request.Okay;
});

/*
 * COMMANDS:
 * 
 * Server commands, things you can type into the server console
 */

context.Add("connections", (args, logger) =>
{
    var view = connection.ContextView();

    if (!view.Connections.Any())
    {
        logger.Log("there is currently zero people connected.");
        return;
    }

    var connections = view.Connections;

    foreach (var conn in connections)
    {
        logger.Log($"[{conn.Client?.Name}, {{uuid: {conn.Client?.UserId}}}] - sessionId: {conn.SessionKey.Value} (expires at {conn.SessionKey.ExpiresAt.ToLongTimeString()})");
    }
});
context.Add("invalidate", (args, logger) =>
{
    if (!(args.Length == 1))
    {
        logger.Log("usage: invalidate <user-id> [will invalidate a users session token]");
        return;
    }

    var uid = args[0];
    var view = connection.ContextView();

    if (!view.TryGetUserById(uid, out var user))
    {
        logger.Log("no user connected with that id");
        return;
    }

    user?.SessionKey.Invalidate();
});
context.Add("mute", (args, logger) =>
{
    if (!(args.Length == 1))
    {
        logger.Log("usage: mute <userid>");
        return;
    }

    if (!connection.ContextView().TryGetUserById(args[0], out var user))
    {
        logger.Log("user does not exist in this context.");
        return;
    }

    user?.Client?.Permissions.Remove("send_message");
});
context.Add("unmute", (args, logger) =>
{
    if (!(args.Length == 1))
    {
        logger.Log("usage: unmute <userid>");
        return;
    }

    if (!connection.ContextView().TryGetUserById(args[0], out var user))
    {
        logger.Log("user does not exist in this context.");
        return;
    }

    user?.Client?.Permissions.Add("send_message");
});

connection.OnMessage = (req, client) =>
{
    connection.Logger?.Log($"received request for '{req.Location}'");

    manager.CallMappedOnline(req.Location, req.Arguments, client);
};

context.Add("user.create", (args, logger) =>
{
    if (!(args.Length == 2))
    {
        logger.Log("usage: user.create <name> <password>");
        return;
    }

    var user = args[0];
    var pass = StorageUtil.Sha256Hash(args[1]);

    connection.AddUser(user, pass);

    logger.Log($"created user. [{user}, {args[1]}]");
});

while (true)
{
    Thread.Sleep(TimeSpan.FromSeconds(.5));
    Console.Write(">> ");
    var input = Console.ReadLine();

    if (string.IsNullOrEmpty(input))
    {
        Console.Clear();
        continue;
    }

    var shattered = input.Split(' ');

    var command = shattered[0];
    List<string> arguments = new();

    if (shattered.Length > 1)
        arguments = shattered[1..].ToList();

    context.Execute(command, arguments.ToArray());
}