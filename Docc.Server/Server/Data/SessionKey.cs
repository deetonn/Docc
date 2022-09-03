namespace Docc.Server.Data;

internal class SessionKey
{
    // defines the default session length.
    private const uint SessionLengthInHours = 4u;

    public SessionKey(DateTime? expiration = null)
    {
        ExpiresAt = expiration ?? DateTime.Now.AddHours(SessionLengthInHours);
    }

    public readonly Guid Value = Guid.NewGuid();
    /// <summary>
    /// The time this session expires, the <see cref="SessionKey"/> will need
    /// to be renewed after this expires.
    /// </summary>
    public DateTime ExpiresAt { get; private set; }

    public bool IsValid
        => ExpiresAt > DateTime.Now;

    /// <summary>
    /// Invalidate the current instance. Who ever has been assigned this key will have to
    /// log back in.
    /// </summary>
    public void Invalidate()
    {
        ExpiresAt = DateTime.Now;
    }
}
