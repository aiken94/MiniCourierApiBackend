namespace CourierBackend.Services.Auth.Interfaces;

public interface ICurrentAdminService
{
    int GetId();

    string? GetEmail();

    string? GetName();

    string? GetRole();

    bool IsAuthenticated();

    bool CanManageAdmin();
}