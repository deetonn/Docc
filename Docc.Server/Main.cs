using static Docc.Common.StaticHelpers;
using Docc.Common;
using Docc.Server;
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Docc.Server.Server;
using Docc.Server.Server.Auth;
using Docc.Common.Storage;

Environment.SetEnvironmentVariable("App-Version", "v0.0.4-dev.1");
Environment.SetEnvironmentVariable("App-Agent", $"Docc {Environment.GetEnvironmentVariable("App-Version")}");

DirectoryListingManager manager = new();
ServerConnection connection = new(ServerType.Local);
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

connection.OnMessage = (req, client) =>
{
    //connection.Logger.Log($"received request for '{req.Location}'");

    var response = manager.CallMappedLocal(req.Location, req.Arguments);
    client.SendRequest(response);
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

    if (shattered.Length > 0)
        arguments = shattered[1..].ToList();

    context.Execute(command, arguments.ToArray());
}