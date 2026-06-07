using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.Application.Common.CQRS;
using PersonalFinance.Application.Common.Interfaces;
using PersonalFinance.Application.UseCases.Auth.Commands;

namespace PersonalFinance.Presentation.Controllers;

[AllowAnonymous]
public class AuthController : ApiControllerBase
{
    private readonly ICommandHandler<RegisterCommand, Guid> _registerHandler;
    private readonly ICommandHandler<LoginCommand, AuthResponse> _loginHandler;
    private readonly ICommandHandler<RefreshTokenCommand, AuthResponse> _refreshTokenHandler;

    public AuthController(
        ICommandHandler<RegisterCommand, Guid> registerHandler,
        ICommandHandler<LoginCommand, AuthResponse> loginHandler,
        ICommandHandler<RefreshTokenCommand, AuthResponse> refreshTokenHandler)
    {
        _registerHandler = registerHandler;
        _loginHandler = loginHandler;
        _refreshTokenHandler = refreshTokenHandler;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command, CancellationToken cancellationToken)
    {
        var result = await _registerHandler.HandleAsync(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
    {
        var result = await _loginHandler.HandleAsync(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var result = await _refreshTokenHandler.HandleAsync(command, cancellationToken);
        return HandleResult(result);
    }
}
