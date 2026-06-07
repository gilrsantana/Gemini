using Moq;
using PersonalFinance.Application.Common.Interfaces;
using PersonalFinance.Application.UseCases.Categories.CommandHandlers;
using PersonalFinance.Application.UseCases.Categories.Commands;
using PersonalFinance.Domain.Entities;

namespace PersonalFinance.UnitTests.Application;

public class UpdateCategoryCommandHandlerTests
{
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly UpdateCategoryCommandHandler _handler;

    public UpdateCategoryCommandHandlerTests()
    {
        _handler = new UpdateCategoryCommandHandler(
            _categoryRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenCommandIsValid()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = Category.Create("Books").Value;

        _categoryRepositoryMock
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        _categoryRepositoryMock
            .Setup(x => x.GetByNameAsync("Novels", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        var command = new UpdateCategoryCommand(categoryId, "Novels");

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Novels", category.Name);

        _categoryRepositoryMock.Verify(x => x.Update(category), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenCategoryDoesNotExist()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        _categoryRepositoryMock
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        var command = new UpdateCategoryCommand(categoryId, "Novels");

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Category.NotFound", result.Error.Code);

        _categoryRepositoryMock.Verify(x => x.Update(It.IsAny<Category>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenCategoryNameIsNotUnique()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = Category.Create("Books").Value;
        var existingCategory = Category.Create("Novels").Value;

        _categoryRepositoryMock
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        _categoryRepositoryMock
            .Setup(x => x.GetByNameAsync("Novels", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        var command = new UpdateCategoryCommand(categoryId, "Novels");

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Category.NameNotUnique", result.Error.Code);

        _categoryRepositoryMock.Verify(x => x.Update(It.IsAny<Category>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenDomainValidationFails()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = Category.Create("Books").Value;

        _categoryRepositoryMock
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        var command = new UpdateCategoryCommand(categoryId, "");

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Category.NameRequired", result.Error.Code);

        _categoryRepositoryMock.Verify(x => x.Update(It.IsAny<Category>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
