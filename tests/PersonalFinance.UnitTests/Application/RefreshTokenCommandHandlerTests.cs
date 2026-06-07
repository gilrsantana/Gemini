using Moq;
using PersonalFinance.Application.Common.Interfaces;
using PersonalFinance.Application.UseCases.Auth.CommandHandlers;
using PersonalFinance.Application.UseCases.Auth.Commands;
using PersonalFinance.Shared;

namespace PersonalFinance.UnitTests.Application;

public class RefreshTokenCommandHandlerTests
{
    private readonly Mock<IIdentityService> _identityServiceMock = new();
    private readonly RefreshTokenCommandHandler _handler;

    public RefreshTokenCommandHandlerTests()
    {
        _handler = new RefreshTokenCommandHandler(_identityServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenTokensAreValid()
    {
        // Arrange
        var command = new RefreshTokenCommand("expired_access_token", "valid_refresh_token");
        var expectedResponse = new AuthResponse("new_access_token", "new_refresh_token", DateTime.UtcNow.AddMinutes(60));

        _identityServiceMock
            .Setup(x => x.RefreshTokenAsync(command.AccessToken, command.RefreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(expectedResponse));

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedResponse, result.Value);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenAccessTokenIsEmpty()
    {
        // Arrange
        var command = new RefreshTokenCommand("", "valid_refresh_token");

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Auth.AccessTokenRequired", result.Error.Code);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenRefreshTokenIsEmpty()
    {
        // Arrange
        var command = new RefreshTokenCommand("expired_access_token", "");

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Auth.RefreshTokenRequired", result.Error.Code);
    }
}
