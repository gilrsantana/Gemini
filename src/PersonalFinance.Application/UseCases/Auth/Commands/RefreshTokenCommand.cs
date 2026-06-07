using PersonalFinance.Application.Common.CQRS;
using PersonalFinance.Application.Common.Interfaces;

namespace PersonalFinance.Application.UseCases.Auth.Commands;

public record RefreshTokenCommand(string AccessToken, string RefreshToken) : ICommand<AuthResponse>;
