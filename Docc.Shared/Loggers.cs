using Pastel;
using System.Drawing;

namespace Docc.Common;

public interface ILogger
{
    void Log<T>(T target, string message);
    void Log<T>(T target, string message, params object[] args);
}

public class ServerConsoleLogger : ILogger
{
    public static string Tab
        => new(' ', 5);

    public void Log<T>(T target, string message)
    {
        var time = DateTime.Now;
        Console.WriteLine($"{time.ToLocalTime().ToLongTimeString()}+{time.Millisecond}  {typeof(T).Name.ToLower().Pastel(Color.LightBlue)}{Tab}{message}");
    }

    public void Log<T>(T target, string message, params object[] args)
    {
        var time = DateTime.Now;
        Console.WriteLine($"{time.ToLocalTime().ToLongTimeString()}+{time.Millisecond}  {typeof(T).Name.ToLower().Pastel(Color.LightBlue)}{Tab}{message}", args);
    }
}
public class ClientConsoleLogger : ILogger
{
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
