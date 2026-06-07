using PersonalFinance.Application.Common.CQRS;
using PersonalFinance.Application.Common.Interfaces;
using PersonalFinance.Application.UseCases.Auth.Commands;
using PersonalFinance.Shared;

namespace PersonalFinance.Application.UseCases.Auth.CommandHandlers;

public class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, AuthResponse>
{
    private readonly IIdentityService _identityService;

    public RefreshTokenCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<AuthResponse>> HandleAsync(RefreshTokenCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.AccessToken))
        {
            return Result.Failure<AuthResponse>(new Error("Auth.AccessTokenRequired", "AccessToken is required."));
        }

        if (string.IsNullOrWhiteSpace(command.RefreshToken))
        {
            return Result.Failure<AuthResponse>(new Error("Auth.RefreshTokenRequired", "RefreshToken is required."));
        }

        return await _identityService.RefreshTokenAsync(command.AccessToken, command.RefreshToken, cancellationToken);
    }
}
