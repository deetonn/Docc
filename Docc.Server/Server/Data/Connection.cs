using Docc.Common.Storage;
using Docc.Server.Server;
using System.Net.Sockets;

namespace Docc.Server.Data;

internal class Connection : IDisposable
{
    public SessionKey? SessionKey { get; init; }
    public Socket? Socket { get; init; }
    public ISafeItem? Client { get; init; }

    public void Dispose()
    {
        Socket?.Dispose();
    }
}
