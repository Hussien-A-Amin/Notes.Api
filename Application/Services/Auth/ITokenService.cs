using Chatting.Api.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Chatting.Api.Application.Services;

public interface ITokenService
{
    Task<TokenResponse> GenerateTokensAsync(
            ApplicationUser user,
            CancellationToken cancellationToken = default);

    Task<TokenResponse?> RefreshTokenAsync(
        RefreshTokenRequest request,
        CancellationToken cancellationToken = default);

    Task RevokeRefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default);
}
public interface IAuthenticationService
{
    Task<TokenResponse> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default);

    Task<TokenResponse> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default);

    Task ForgotPasswordAsync(
        ForgotPasswordRequest request,
        CancellationToken cancellationToken = default);

    Task ResetPasswordAsync(
        ResetPasswordRequest request,
        CancellationToken cancellationToken = default);

    Task ConfirmEmailAsync(
        ConfirmEmailRequest request,
        CancellationToken cancellationToken = default);

    Task ChangePasswordAsync(
        string userId,
        ChangePasswordRequest request,
        CancellationToken cancellationToken = default);
}
public sealed class AuthenticationService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    ITokenService tokenService) : IAuthenticationService
{
    public async Task<TokenResponse> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email
        };

        var result = await userManager.CreateAsync(
            user,
            request.Password);

        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join(
                    "; ",
                    result.Errors.Select(x => x.Description)));
        }

        return await tokenService.GenerateTokensAsync(
            user,
            cancellationToken);
    }

    public async Task<TokenResponse> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
            throw new UnauthorizedAccessException(
                "Invalid email or password.");

        var result = await signInManager.CheckPasswordSignInAsync(
            user,
            request.Password,
            lockoutOnFailure: true);

        if (!result.Succeeded)
            throw new UnauthorizedAccessException(
                "Invalid email or password.");

        return await tokenService.GenerateTokensAsync(
            user,
            cancellationToken);
    }

    public async Task ForgotPasswordAsync(
        ForgotPasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
            return;

        var token = await userManager.GeneratePasswordResetTokenAsync(user);

        // TODO: Send reset token through IEmailService.
    }

    public async Task ResetPasswordAsync(
        ResetPasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)        
            throw new InvalidOperationException(
                "Unable to reset password.");

        var result = await userManager.ResetPasswordAsync(
            user,
            request.ResetCode,
            request.NewPassword);

        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join(
                    "; ",
                    result.Errors.Select(x => x.Description)));
        }
    }

    public async Task ConfirmEmailAsync(
        ConfirmEmailRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(request.UserId);

        if (user is null)
            throw new InvalidOperationException(
                "Unable to confirm email.");

        var result = await userManager.ConfirmEmailAsync(
            user,
            request.Token);

        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join(
                    "; ",
                    result.Errors.Select(x => x.Description)));
        }
    }

    public async Task ChangePasswordAsync(
        string userId,
        ChangePasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId);

        if (user is null)
            throw new UnauthorizedAccessException();

        var result = await userManager.ChangePasswordAsync(
            user,
            request.CurrentPassword,
            request.NewPassword);

        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join(
                    "; ",
                    result.Errors.Select(x => x.Description)));
        }
    }
}
public class RefreshToken
{
    public Guid Id { get; set; }

    public string UserId { get; set; } = default!;

    public string TokenHash { get; set; } = default!;

    public DateTime ExpiresAtUtc { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? RevokedAtUtc { get; set; }

    public string? ReplacedByTokenHash { get; set; }

    public bool IsExpired =>
        DateTime.UtcNow >= ExpiresAtUtc;

    public bool IsRevoked =>
        RevokedAtUtc.HasValue;

    public bool IsActive =>
        !IsExpired && !IsRevoked;
}
public sealed class JwtOptions
{
    

    public string Issuer { get; set; } = default!;

    public string Audience { get; set; } = default!;

    public string SigningKey { get; set; } = default!;

    public int AccessTokenExpirationMinutes { get; set; } = 15;

    public int RefreshTokenExpirationDays { get; set; } = 7;
}
public sealed class JwtTokenService(
    UserManager<ApplicationUser> userManager,
    IOptions<JwtOptions> jwtOptions) : ITokenService
{
    private readonly JwtOptions _options = jwtOptions.Value;

    public async Task<TokenResponse> GenerateTokensAsync(
        ApplicationUser user,
        CancellationToken cancellationToken = default)
    {
        var roles = await userManager.GetRolesAsync(user);

        var accessToken = GenerateAccessToken(
            user,
            roles);

        var refreshToken = GenerateRefreshToken();

        // TODO:
        // Persist refreshToken hash in database.

        return new TokenResponse(
            accessToken.Token,
            refreshToken,
            accessToken.ExpiresAtUtc);
    }

    public async Task<TokenResponse?> RefreshTokenAsync(
        RefreshTokenRequest request,
        CancellationToken cancellationToken = default)
    {
        // TODO:
        // 1. Hash incoming refresh token
        // 2. Find it in database
        // 3. Check expiration
        // 4. Check revocation
        // 5. Revoke old token
        // 6. Generate new access token
        // 7. Generate new refresh token
        // 8. Persist new refresh token

        await Task.CompletedTask;

        return null;
    }

    public async Task RevokeRefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        // TODO:
        // Hash token
        // Find stored token
        // Mark it as revoked

        await Task.CompletedTask;
    }


    private (string Token, DateTime ExpiresAtUtc) GenerateAccessToken(
        ApplicationUser user,
        IList<string> roles)
    {
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(
            _options.AccessTokenExpirationMinutes);

        var claims = new List<Claim>
        {
            new (ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(ClaimTypes.Name, user.UserName ?? string.Empty)
        };

        foreach (var role in roles)
        {
            claims.Add(
                new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_options.SigningKey));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler()
            .WriteToken(token);

        return (tokenString, expiresAtUtc);
    }


    private static string GenerateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);

        return Convert.ToBase64String(randomBytes);
    }
}