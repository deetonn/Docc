using Docc.Common;

namespace Docc.Server.Server;

internal class CommandLogger : ILogger
{
    public string Target => "ServerContext";

    public static string Tab
        => new(' ', 5);

    public void Log(string message)
    {
        Console.WriteLine($"{DateTime.Now.ToLongTimeString()} {Target}{Tab}{message}");
    }

    public void Log(string message, params object[] args)
    {
        Console.WriteLine($"{DateTime.Now.ToLongTimeString()} {Target}\t{message}", args);

    }
}
