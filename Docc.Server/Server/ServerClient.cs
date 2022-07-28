using Docc.Common.Data;

namespace Docc.Server.Server;

public class ServerClient
{
    // add any client information that is
    // server specific.

    public string? Name { get; init; }
    public Guid Id { get; init; }
    public bool IsAdmin { get; set; } = false;

    public static ServerClient From(SharedClient client)
    {
        return new()
        {
            Name = client.Name,
            Id = client.Id,
        };
    }
}
