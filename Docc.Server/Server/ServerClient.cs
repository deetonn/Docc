using Docc.Common.Data;

namespace Docc.Server.Server;

// TODO:

/*
 * This will likely need an entire overhaul.
 * 
 * The plan to have a server context object that associates
 * SessionID -> User -> Socket
 * where SessionID is generated when a user connects
 * does not align with taking the data from the client.
 * 
 * Once we have a user that has been assigned a SessionID & have been validated,
 * this object is viable.
 * 
 * ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
 * Ramblings
 */

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
