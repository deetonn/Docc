using Docc.Common.Storage;
using Docc.Server.Server;
using System.Net.Sockets;

namespace Docc.Server.Data;

internal class Connection : IDisposable
{
    // this shouldn't be nullable.
    // any error must be emmited because
    // every user should at all times have a session id.
    public SessionKey SessionKey { get; init; }
    public Socket? Socket { get; init; }
    public ISafeItem? Client { get; init; }

    public void Dispose()
    {
        Socket?.Dispose();
    }
}
