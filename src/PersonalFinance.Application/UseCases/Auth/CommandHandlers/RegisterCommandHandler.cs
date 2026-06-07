using PersonalFinance.Application.Common.CQRS;
using PersonalFinance.Application.Common.Interfaces;
using PersonalFinance.Application.UseCases.Auth.Commands;
using PersonalFinance.Shared;

namespace PersonalFinance.Application.UseCases.Auth.CommandHandlers;

public class RegisterCommandHandler : ICommandHandler<RegisterCommand, Guid>
{
    private readonly IIdentityService _identityService;

    public RegisterCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<Guid>> HandleAsync(RegisterCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.Email))
        {
            return Result.Failure<Guid>(new Error("Auth.EmailRequired", "Email is required."));
        }

        if (string.IsNullOrWhiteSpace(command.Password))
        {
            return Result.Failure<Guid>(new Error("Auth.PasswordRequired", "Password is required."));
        }

        return await _identityService.RegisterAsync(command.Email, command.Password, cancellationToken);
    }
}
