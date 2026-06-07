using PersonalFinance.Application.Common.CQRS;
using PersonalFinance.Application.Common.Interfaces;
using PersonalFinance.Application.UseCases.Auth.Commands;
using PersonalFinance.Shared;

namespace PersonalFinance.Application.UseCases.Auth.CommandHandlers;

public class LoginCommandHandler : ICommandHandler<LoginCommand, AuthResponse>
{
    private readonly IIdentityService _identityService;

    public LoginCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<AuthResponse>> HandleAsync(LoginCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.Email))
        {
            return Result.Failure<AuthResponse>(new Error("Auth.EmailRequired", "Email is required."));
        }

        if (string.IsNullOrWhiteSpace(command.Password))
        {
            return Result.Failure<AuthResponse>(new Error("Auth.PasswordRequired", "Password is required."));
        }

        return await _identityService.LoginAsync(command.Email, command.Password, cancellationToken);
    }
}
