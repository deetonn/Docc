using Docc.Server.Data;

namespace Docc.Common.Auth;

/// <summary>
/// Provides a generic authorization service.
/// Must contain a parameterless constructor 
/// </summary>
internal interface IAuthorizationService
{
    public bool Authorize(Connection @user);
}
