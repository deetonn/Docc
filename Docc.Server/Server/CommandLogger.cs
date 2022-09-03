using Docc.Common;
using Pastel;
using System.Drawing;

namespace Docc.Server.Server;

internal class CommandLogger : ILogger
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
