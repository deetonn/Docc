using Docc.Common;
using Docc.Common.Data;
using Docc.Common.Auth;

namespace Docc.Server;

internal class PrivateServerAuthorization : IAuthorizationService
{
    public bool Authorize(SharedClient user)
    {
        /*
         * obviously a silly example,
         * but literally anything could be here.
         * 
         * enough modification could allow 'user'
         * to contain data needed to verify the
         * connection.
         */
        return user.Name.StartsWith("@admin.");
    }
}
internal class NoServerAuthorization : IAuthorizationService
{
    public bool Authorize(SharedClient _)
    {
        /*
         * obviously a silly example,
         * but literally anything could be here.
         * 
         * enough modification could allow 'user'
         * to contain data needed to verify the
         * connection.
         */

        return true;
    }
}
