using static Docc.Common.StaticHelpers;
using Docc.Common;
using Docc.Server;
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Net.Sockets;

Environment.SetEnvironmentVariable("App-Version", "v0.0.4-dev.1");
Environment.SetEnvironmentVariable("App-Agent", $"Docc {Environment.GetEnvironmentVariable("App-Version")}");

DirectoryListingManager manager = new();
ServerConnection connection = new();
CommandList context = new(new CommandLogger());

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

Random serverRng = new();
var version = Environment.GetEnvironmentVariable("App-Version")!;

manager.MapGet("/util/random", (args, _) =>
{
    int min = 0, max = 0xFFFF;
    return Builder()
        .WithLocation(Request.DefaultLocation)
        .WithResult(Okay())
        .AddContent(serverRng.Next(min, max).ToString())
        .Build();
});

// An example of verifying, bearing in mind I've no clue how to verify someone
manager.MapSet("/admin/verify", (args, sender) =>
{
    var failure =
        new RequestBuilder()
            .WithLocation(Request.DefaultLocation)
            .WithResult(RequestResult.NotAuthorized)
            .AddContent("You are not authorized.")
            .Build();

    if (!args.ContainsKey("uuid"))
        return failure;
    if (!args.ContainsKey("apiKey"))
        return failure;

    var apiKey = args["apiKey"];
    var Uuid = args["uuid"];

    if (!connection.Connections.Any(x => x.Key.Id.ToString() == Uuid))
        return failure;

    if (apiKey != "249995ce-1683-4fc5-83e0-5628de154fe3")
    {
        return failure;
    }

    connection.EditUser(Uuid, user =>
    {
        user.IsAdmin = true;
        return user;
    });

    return new RequestBuilder()
        .WithLocation(Request.DefaultLocation)
        .WithResult(Okay())
        .AddContent("Authorized!")
        .Build();

});
manager.MapSet("/redeem", (args, sender) =>
{
    var rejection = new RequestBuilder()
        .WithLocation(Request.DefaultLocation)
        .WithResult(RequestResult.BadArguments)
        .AddContent($"redeem expects once argument 'key'. The key to redeem. It must be a valid key.");

    if (!args.ContainsKey("key"))
        return rejection.Build();

    var key = args["key"];

    if (!Guid.TryParse(key, out var redeemKey))
    {
        return rejection
            .AddContent("the key has an invalid format.")
            .Build();
    }

    if (!context.ActiveKeys.ContainsKey(redeemKey))
        return rejection
            .AddContent("the key does not exist or has been redeemed.")
            .Build();

    var giftcard = context.ActiveKeys[redeemKey];
    context.ActiveKeys.Remove(redeemKey);
    return new RequestBuilder()
        .WithLocation(Request.DefaultLocation)
        .WithResult(Okay())
        .AddContent($"succesfully redeemed {giftcard.Amount}x {giftcard.Name}!")
        .Build();
});

manager.MapGet("/api/v1/version", (_, _) =>
{
    return new RequestBuilder()
        .WithLocation(Request.DefaultLocation)
        .WithResult(Okay())
        .AddContent(version)
        .Build();
});

context.Add("version.set", (args, logger) =>
{
    if (args.Length != 1)
    {
        logger?.Log($"expected 1 argument");
        return;
    }

    version = args[0];
});

context.Add("endpoint.test", (args, logger) =>
{
    if (!args.Any())
    {
        logger.Log($"endpoint.test expects one argument: endpoint");
        return;
    }

    var endpoint = args.First();
    var response = manager.CallMappedLocal(endpoint, new());

    logger.Log("\n" + response.ToString() + "\n");
});
context.Add("endpoint.list", (args, logger) =>
{
    logger.Log($"{string.Join(", ", manager.ActiveListings)}");
});
context.Add("endpoint.dummy", (args, logger) =>
{
    if (!args.Any())
    {
        logger.Log($"to create a dummy endpoint, you must specify it's name.");
        return;
    }

    var dummy = args.First();
    manager.MapGet(dummy, (args, sender) =>
    {
        logger.Log($"dummy endpoint hit '{dummy}'");
        logger.Log($"endpoint result\n");

        return Builder()
            .WithLocation($"/user?uuid=this&hitDummy=true")
            .WithResult(Okay())
            .AddContent($"You've been banned until {DateTime.Now.AddDays(365 * 25).ToLongDateString()} (25 years).")
            .Build();
    });
});

context.Add("server.connections", (args, logger) =>
{
    if (connection.Connections.Count == 0)
    {
        logger.Log($"there is nobody connected.");
        return;
    }

    StringBuilder @conn = new();

    conn.AppendLine($"\n{connection.Connections.Count} connections\n");

    foreach (var client in connection.Connections)
    {
        @conn.AppendLine($"({client.Key.Id}, {client.Key.Name}) - available: {client.Value.Available}, connected: {client.Value.Connected}");
    }

    logger.Log(conn.ToString());
});

connection.OnMessage = (req, client) =>
{
    connection.Logger.Log($"received request for '{req.Location}'");

    var response = manager.CallMappedLocal(req.Location, req.Arguments);
    client.SendRequest(response);
};

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

    if (shattered.Length > 0)
        arguments = shattered[1..].ToList();

    context.Execute(command, arguments.ToArray());
}