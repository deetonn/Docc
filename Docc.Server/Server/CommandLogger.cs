using Docc.Common;

namespace Docc.Server.Server;

internal class CommandLogger : ILogger
{
    public string Target => "ServerContext";

    public static string Tab
        => new(' ', 5);

    public void Log<T>(T target, string message)
    {
        Console.WriteLine($"{DateTime.Now.ToLongTimeString()} {typeof(T).Name}{Tab}{message}");
    }

    public void Log<T>(T target, string message, params object[] args)
    {
        Console.WriteLine($"{DateTime.Now.ToLongTimeString()} {typeof(T).Name}\t{message}", args);

    }
}
