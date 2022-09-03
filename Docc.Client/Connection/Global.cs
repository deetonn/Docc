using Docc.Common;
using Newtonsoft.Json;

using System.Windows.Forms;

namespace Docc.Client.Connection;

[JsonObject]
public class LocalLoginCredentials
{
    [JsonProperty(PropertyName = "local-user")]
    public string Username { get; set; }

    [JsonProperty(PropertyName = "creds")]
    public string HashedPassword { get; set; }
}

public static class Global
{
    private const string logFile = "docc-logs.txt";

    static Global()
    {
        if (File.Exists(logFile))
            File.Delete(logFile);
    }

    public static Client? Client { get; set; }
    public static Dictionary<string, Action<Request>> Handlers = new();

    public static (bool, string) TryLogin(string user, string pass, bool raw = false)
    {
        Client conn = null!;
        bool res = false;
        string msg = string.Empty;

        try
        {
            (res, msg) = DoccClient.Create(user, pass, out conn, raw);
        }
        catch (ObjectDisposedException)
        {
            msg = "cannot connect to the server.";
        }

        Client = conn;
        return (res, msg);
    }
    public static (bool, string) TryLoginFromSaveFile(string saveFile)
    {
        if (!File.Exists(saveFile))
        {
            return (false, "save file not found.");
        }

        LocalLoginCredentials? creds;

        try
        {
            var contents = File.ReadAllText(saveFile);
            creds = JsonConvert.DeserializeObject<LocalLoginCredentials>(contents);
        }
        catch (JsonException e)
        {
            return (false, e.Message);
        }

        if (creds is LocalLoginCredentials login)
        {
            return TryLogin(login.Username, login.HashedPassword, true);
        }

        return (false, "failed to load saved login information");
    }

    public static void SaveLogin(LocalLoginCredentials creds, string saveFile)
    {
        var json = JsonConvert.SerializeObject(creds);
        File.WriteAllText(saveFile, json);
    }


    public static void AddCallback(string loc, Action<Request> req)
    {
        Handlers.Add(loc, req);
    }


    public static void Log(string message)
    {
        if (!File.Exists(logFile))
        {
            File.Create(logFile).Close();
        }

        File.AppendAllText(logFile, message);
    }

    public static string[] LoadLogFileText()
    {
        if (!File.Exists(logFile))
            return new[] { "no logs yet." };

        return File.ReadAllLines(logFile);
    }
}
