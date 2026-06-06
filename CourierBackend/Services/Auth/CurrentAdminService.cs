using System.Security.Claims;
using CourierBackend.Services.Auth.Interfaces;

namespace CourierBackend.Services.Auth;

public class CurrentAdminService : ICurrentAdminService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentAdminService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int GetId()
    {
        var claim = _httpContextAccessor
            .HttpContext?
            .User
            .FindFirst(ClaimTypes.NameIdentifier);

        if (claim is null)
        {
            throw new UnauthorizedAccessException(
                "User is not authenticated.");
        }

        return int.Parse(claim.Value);
    }

    public string? GetEmail()
    {
        return _httpContextAccessor
            .HttpContext?
            .User
            .FindFirst(ClaimTypes.Email)?
            .Value;
    }

    public string? GetName()
    {
        return _httpContextAccessor
            .HttpContext?
            .User
            .FindFirst(ClaimTypes.Name)?
            .Value;
    }

    public string? GetRole()
    {
        return _httpContextAccessor
            .HttpContext?
            .User
            .FindFirst(ClaimTypes.Role)?
            .Value;
    }

    public bool IsAuthenticated()
    {
        return _httpContextAccessor
            .HttpContext?
            .User
            ?.Identity
            ?.IsAuthenticated ?? false;
    }

    public bool CanManageAdmin()
    {
        if (GetRole() == "Admin")
        {
            return true;
        }

        return false;
    }
}