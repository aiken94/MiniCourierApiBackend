using CourierBackend.Data.Requests.Auth;
using CourierBackend.Models;

namespace CourierBackend.Services.Auth.Interfaces;

public interface IJwtService
{
    Task<LoginResource> GenerateTokensAsync(Admin admin);

    Task<LoginResource?> RefreshTokenAsync(string refreshToken);

    Task ForgotPasswordAsync(string email);

    Task ResetPasswordAsync(string token, string newPassword);
}