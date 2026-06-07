using Moq;
using PersonalFinance.Application.Common.Interfaces;
using PersonalFinance.Application.UseCases.Categories.CommandHandlers;
using PersonalFinance.Application.UseCases.Categories.Commands;
using PersonalFinance.Domain.Entities;

namespace PersonalFinance.UnitTests.Application;

public class CreateCategoryCommandHandlerTests
{
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly CreateCategoryCommandHandler _handler;

    public CreateCategoryCommandHandlerTests()
    {
        _handler = new CreateCategoryCommandHandler(
            _categoryRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreateCategoryCommand("Books");

        _categoryRepositoryMock
            .Setup(x => x.GetByNameAsync(command.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);

        _categoryRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenCategoryNameIsNotUnique()
    {
        // Arrange
        var command = new CreateCategoryCommand("Books");
        var existingCategory = Category.Create("Books").Value;

        _categoryRepositoryMock
            .Setup(x => x.GetByNameAsync(command.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Category.NameNotUnique", result.Error.Code);

        _categoryRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenDomainValidationFails()
    {
        // Arrange
        var command = new CreateCategoryCommand("");

        _categoryRepositoryMock
            .Setup(x => x.GetByNameAsync(command.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Category.NameRequired", result.Error.Code);

        _categoryRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
