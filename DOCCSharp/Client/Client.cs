using Docc.Common;

namespace Docc.Client;

// This is my personal client, so the name will always be Admin
// and the userId will always be spoofed.

internal class Client
{
    // this is not a real API key, literally just a random Guid
    const string ApiKey = "249995ce-1683-4fc5-83e0-5628de154fe3";
    public ClientConnection Connection { get; }
    protected ILogger? _logger;

    public void UseLogger<T>() where T : class, ILogger, new()
    {
        _logger = new T();
    }

    public Client(string user, string pw)
    {
        var sessionUid = Guid.NewGuid();
        Connection = new ClientConnection(user, pw);
    }

    public Request? MakeRequest(Request req)
    {
        var sessionId = Connection.SessionId;
        req.Arguments.Add("session_id", sessionId);
        return Connection.MakeRequest(req);
    }
}
