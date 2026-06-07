using Moq;
using PersonalFinance.Application.Common.Interfaces;
using PersonalFinance.Application.UseCases.Auth.CommandHandlers;
using PersonalFinance.Application.UseCases.Auth.Commands;
using PersonalFinance.Shared;

namespace PersonalFinance.UnitTests.Application;

public class LoginCommandHandlerTests
{
    private readonly Mock<IIdentityService> _identityServiceMock = new();
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _handler = new LoginCommandHandler(_identityServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenCredentialsAreValid()
    {
        // Arrange
        var command = new LoginCommand("test@test.com", "Password123!");
        var expectedResponse = new AuthResponse("access_token", "refresh_token", DateTime.UtcNow.AddMinutes(60));

        _identityServiceMock
            .Setup(x => x.LoginAsync(command.Email, command.Password, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(expectedResponse));

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedResponse, result.Value);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenEmailIsEmpty()
    {
        // Arrange
        var command = new LoginCommand("", "Password123!");

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Auth.EmailRequired", result.Error.Code);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenPasswordIsEmpty()
    {
        // Arrange
        var command = new LoginCommand("test@test.com", "");

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Auth.PasswordRequired", result.Error.Code);
    }
}
