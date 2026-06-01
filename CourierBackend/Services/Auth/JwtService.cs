using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CourierBackend.Configurations.Services;
using CourierBackend.Services.Auth.Interfaces;
using CourierBackend.Data.Requests.Auth;
using CourierBackend.Data.Resources;
using CourierBackend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using CourierBackend.Data;
using Microsoft.AspNetCore.Identity;
using CourierBackend.Services.Email.Interfaces;
using CourierBackend.Services.Email.Templates;

namespace CourierBackend.Services.Auth;

public class JwtService : IJwtService
{
    private readonly JwtSettings _jwtSettings;
    private readonly CourierContext _context;
    private readonly IEmailService _emailService;
    private readonly PasswordHasher<object> _passwordHasher;

    public JwtService(IOptions<JwtSettings> jwtSettings, IEmailService emailService, CourierContext context)
    {
        _jwtSettings = jwtSettings.Value;
        _context = context;
        _emailService = emailService;

        _passwordHasher = new PasswordHasher<object>();
    }

    public async Task<LoginResource> GenerateTokensAsync(Admin admin)
    {
        var claims = new List<Claim>{
            new(ClaimTypes.NameIdentifier, admin.Id.ToString()),
            new(ClaimTypes.Name, admin.Name),
            new(ClaimTypes.Email, admin.Email),
            new(ClaimTypes.Role, admin.Role.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials
        );

        var accessToken = new JwtSecurityTokenHandler()
            .WriteToken(token);

        var refreshToken = _GenerateRefreshToken();

        var refreshTokenEntity = new PersonalAccessToken
        {
            Id = Guid.NewGuid(),
            Token = refreshToken,
            AdminId = admin.Id,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays)
        };

        _context.PersonalAccessTokens.Add(refreshTokenEntity);

        await _context.SaveChangesAsync();

        return new LoginResource
        {
            Admin = AdminResource.FromModel(admin),
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = expires
        };
    }

    public async Task<LoginResource?> RefreshTokenAsync(string refreshToken)
    {
        var tokenEntity = await _context.PersonalAccessTokens
            .Include(r => r.Admin)
            .FirstOrDefaultAsync(r =>
                r.Token == refreshToken &&
                !r.IsRevoked);

        if (tokenEntity is null)
        {
            return null;
        }

        if (tokenEntity.ExpiresAt < DateTime.UtcNow)
        {
            return null;
        }

        tokenEntity.IsRevoked = true;

        await _context.SaveChangesAsync();

        return await GenerateTokensAsync(tokenEntity.Admin);
    }

    public async Task ForgotPasswordAsync(string email)
    {
        var admin = await _context.Admins
            .FirstOrDefaultAsync(x => x.Email == email);

        // Prevent email enumeration
        if (admin is null)
        {
            return;
        }

        var rawToken = _GenerateSecureToken();

        var tokenHash = _HashToken(rawToken);

        var resetToken = new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            AdminId = admin.Id,
            TokenHash = tokenHash,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(30)
        };

        _context.PasswordResetTokens.Add(resetToken);

        await _context.SaveChangesAsync();

        var resetLink = $"https://admin.courierbackend.com/reset-password?token={Uri.EscapeDataString(rawToken)}";

        var html = PasswordResetTemplate.Build(admin.Name, resetLink);

        await _emailService.SendAsync(
            admin.Email,
            "Reset Your Password",
            html);

        await _emailService.SendAsync(admin.Email, "Password Reset", html);
    }

    public async Task ResetPasswordAsync(string token, string newPassword)
    {
        var tokenHash = _HashToken(token);

        var resetToken = await _context.PasswordResetTokens
            .Include(x => x.Admin)
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash && !x.IsUsed);

        if (resetToken is null)
        {
            throw new Exception("Invalid reset token.");
        }

        if (resetToken.ExpiresAt < DateTime.UtcNow)
        {
            throw new Exception("Reset token expired.");
        }

        resetToken.Admin.PasswordHash = _passwordHasher.HashPassword(new object(), newPassword);

        resetToken.IsUsed = true;

        // Revoke all access tokens
        var accessTokens = await _context.PersonalAccessTokens
            .Where(x => x.AdminId == resetToken.AdminId && !x.IsRevoked)
            .ToListAsync();

        foreach (var accessToken in accessTokens)
        {
            accessToken.IsRevoked = true;
        }

        await _context.SaveChangesAsync();
    }

    private static string _GenerateRefreshToken()
    {
        var randomBytes = new byte[64];

        using var rng = RandomNumberGenerator.Create();

        rng.GetBytes(randomBytes);

        return Convert.ToBase64String(randomBytes);
    }

    private static string _GenerateSecureToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);

        return Convert.ToBase64String(bytes);
    }

    private static string _HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));

        return Convert.ToHexString(bytes);
    }
}