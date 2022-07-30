using Docc.Common;
using Docc.Common.Data;
using Docc.Common.Auth;
using Docc.Server.Data;

namespace Docc.Server.Server.Auth;

// TODO:

/*
 * Not sure how viable this will be as it stands, maybe 'Docc.Shared.Auth.IAuthorizationService'
 * will need to take in the users credentials and validate via that.
 */

internal class PrivateServerAuthorization : IAuthorizationService
{
    public bool Authorize(Connection user)
    {
        /*
         * obviously a silly example,
         * but literally anything could be here.
         * 
         * enough modification could allow 'user'
         * to contain data needed to verify the
         * connection.
         */
        return user.Client?.Name?.StartsWith("@admin.") ?? false;
    }
}
internal class NoServerAuthorization : IAuthorizationService
{
    public bool Authorize(Connection _)
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
