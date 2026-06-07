using PersonalFinance.Application.Common.CQRS;
using PersonalFinance.Application.Common.Interfaces;

namespace PersonalFinance.Application.UseCases.Auth.Commands;

public record LoginCommand(string Email, string Password) : ICommand<AuthResponse>;
