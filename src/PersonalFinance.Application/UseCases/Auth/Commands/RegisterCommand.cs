using PersonalFinance.Application.Common.CQRS;

namespace PersonalFinance.Application.UseCases.Auth.Commands;

public record RegisterCommand(string Email, string Password) : ICommand<Guid>;
