using Docc.Common;

namespace Docc.Client;

public class Client
{
    public ClientConnection Connection { get; }
    protected ILogger? _logger;

    public void UseLogger<T>() where T : class, ILogger, new()
    {
        _logger = new T();
    }

    public Client(string user, string pw, bool raw)
    {
        var sessionUid = Guid.NewGuid();
        Connection = new ClientConnection(user, pw, raw);
    }

    public Request? MakeRequest(Request req)
    {
        var sessionId = Connection.SessionId;
        req.Arguments.Add("session_id", sessionId);
        return Connection.MakeRequest(req);
    }

    public void MakeSpurnRequest(Request data)
    {
        var sessionId = Connection.SessionId;
        data.Arguments.Add("session_id", sessionId);
        Connection.MakeSpurnRequest(data);
    }
}
