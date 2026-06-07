using Moq;
using PersonalFinance.Application.Common.Interfaces;
using PersonalFinance.Application.UseCases.Auth.CommandHandlers;
using PersonalFinance.Application.UseCases.Auth.Commands;
using PersonalFinance.Shared;

namespace PersonalFinance.UnitTests.Application;

public class RegisterCommandHandlerTests
{
    private readonly Mock<IIdentityService> _identityServiceMock = new();
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _handler = new RegisterCommandHandler(_identityServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenCredentialsAreValid()
    {
        // Arrange
        var command = new RegisterCommand("test@test.com", "Password123!");
        var expectedUserId = Guid.NewGuid();

        _identityServiceMock
            .Setup(x => x.RegisterAsync(command.Email, command.Password, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(expectedUserId));

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedUserId, result.Value);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenEmailIsEmpty()
    {
        // Arrange
        var command = new RegisterCommand("", "Password123!");

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
        var command = new RegisterCommand("test@test.com", "");

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Auth.PasswordRequired", result.Error.Code);
    }
}
