using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Docc.Server.Data;

internal class SessionKey
{
    // defines the default session length.
    private const int SessionLengthInHours = 4;

    public SessionKey(DateTime? expiration = null)
    {
        ExpiresAt = expiration ?? DateTime.Now.AddHours(SessionLengthInHours);
    }

    public readonly Guid Value = Guid.NewGuid();
    /// <summary>
    /// The time this session expires, the <see cref="SessionKey"/> will need
    /// to be renewed after this expires.
    /// </summary>
    public DateTime ExpiresAt { get; }

    public bool IsValid()
        => ExpiresAt < DateTime.Now;
}
