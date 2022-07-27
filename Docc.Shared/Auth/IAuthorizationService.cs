using Docc.Common.Data;

namespace Docc.Common.Auth;

/// <summary>
/// Provides a generic authorization service.
/// Must contain a parameterless constructor 
/// </summary>
public interface IAuthorizationService
{
    public bool Authorize(SharedClient @user);
}
