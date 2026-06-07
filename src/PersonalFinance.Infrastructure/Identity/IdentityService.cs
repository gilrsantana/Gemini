using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PersonalFinance.Application.Common.Interfaces;
using PersonalFinance.Shared;

namespace PersonalFinance.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<Account> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly JwtSettings _jwtSettings;

    public IdentityService(
        UserManager<Account> userManager,
        RoleManager<Role> roleManager,
        IOptions<JwtSettings> jwtSettings)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<Result<Guid>> RegisterAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var existingAccount = await _userManager.FindByEmailAsync(email);
        if (existingAccount != null)
        {
            return Result.Failure<Guid>(new Error("Auth.EmailExists", "An account with this email already exists."));
        }

        var account = Account.Create(Guid.NewGuid(), email);
        var result = await _userManager.CreateAsync(account, password);
        if (!result.Succeeded)
        {
            var error = result.Errors.FirstOrDefault();
            return Result.Failure<Guid>(new Error("Auth.RegistrationFailed", error?.Description ?? "Failed to create user."));
        }

        return Result.Success(account.Id);
    }

    public async Task<Result<AuthResponse>> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var account = await _userManager.FindByEmailAsync(email);
        if (account == null)
        {
            return Result.Failure<AuthResponse>(new Error("Auth.InvalidCredentials", "Invalid email or password."));
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(account, password);
        if (!isPasswordValid)
        {
            return Result.Failure<AuthResponse>(new Error("Auth.InvalidCredentials", "Invalid email or password."));
        }

        if (await _userManager.IsLockedOutAsync(account))
        {
            return Result.Failure<AuthResponse>(new Error("Auth.LockedOut", "This account has been locked out."));
        }

        var roles = await _userManager.GetRolesAsync(account);
        var accessToken = GenerateAccessToken(account, roles);
        var refreshToken = GenerateRefreshToken();

        var expiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryInDays);
        account.UpdateRefreshToken(refreshToken, expiryTime);

        var updateResult = await _userManager.UpdateAsync(account);
        if (!updateResult.Succeeded)
        {
            return Result.Failure<AuthResponse>(new Error("Auth.UpdateFailed", "Failed to update account session."));
        }

        return Result.Success(new AuthResponse(accessToken, refreshToken, expiryTime));
    }

    public async Task<Result<AuthResponse>> RefreshTokenAsync(string accessToken, string refreshToken, CancellationToken cancellationToken = default)
    {
        var principal = GetPrincipalFromExpiredToken(accessToken);
        if (principal == null)
        {
            return Result.Failure<AuthResponse>(new Error("Auth.InvalidToken", "Invalid access token."));
        }

        var emailClaim = principal.FindFirst(ClaimTypes.Email) ?? principal.FindFirst(JwtRegisteredClaimNames.Email);
        if (emailClaim == null)
        {
            return Result.Failure<AuthResponse>(new Error("Auth.InvalidToken", "Invalid access token claims."));
        }

        var account = await _userManager.FindByEmailAsync(emailClaim.Value);
        if (account == null || account.RefreshToken != refreshToken || account.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return Result.Failure<AuthResponse>(new Error("Auth.InvalidRefreshToken", "Invalid or expired refresh token."));
        }

        var roles = await _userManager.GetRolesAsync(account);
        var newAccessToken = GenerateAccessToken(account, roles);
        var newRefreshToken = GenerateRefreshToken();

        var expiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryInDays);
        account.UpdateRefreshToken(newRefreshToken, expiryTime);

        var updateResult = await _userManager.UpdateAsync(account);
        if (!updateResult.Succeeded)
        {
            return Result.Failure<AuthResponse>(new Error("Auth.UpdateFailed", "Failed to update account session."));
        }

        return Result.Success(new AuthResponse(newAccessToken, newRefreshToken, expiryTime));
    }

    private string GenerateAccessToken(Account account, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, account.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, account.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expiry = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes > 0 ? _jwtSettings.ExpiryInMinutes : 60);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expiry,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || 
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }
            return principal;
        }
        catch
        {
            return null;
        }
    }
}
