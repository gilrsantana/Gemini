using PersonalFinance.Shared;

namespace PersonalFinance.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<Result<Guid>> RegisterAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<Result<AuthResponse>> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<Result<AuthResponse>> RefreshTokenAsync(string accessToken, string refreshToken, CancellationToken cancellationToken = default);
}

public record AuthResponse(string AccessToken, string RefreshToken, DateTime ExpiryTime);
