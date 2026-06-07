using Moq;
using PersonalFinance.Application.Common.Interfaces;
using PersonalFinance.Application.UseCases.Products.CommandHandlers;
using PersonalFinance.Application.UseCases.Products.Commands;
using PersonalFinance.Domain.Entities;

namespace PersonalFinance.UnitTests.Application;

public class CreateProductCommandHandlerTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock = new();
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly CreateProductCommandHandler _handler;

    private static readonly Guid CategoryId = Guid.NewGuid();

    public CreateProductCommandHandlerTests()
    {
        _handler = new CreateProductCommandHandler(
            _productRepositoryMock.Object, 
            _categoryRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreateProductCommand("Valid Product", 99.90m, CategoryId);
        var category = Category.Create("Valid Category").Value;

        _categoryRepositoryMock
            .Setup(x => x.GetByIdAsync(CategoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        _productRepositoryMock
            .Setup(x => x.GetByNameAsync(command.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);
        
        _productRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenCategoryDoesNotExist()
    {
        // Arrange
        var command = new CreateProductCommand("Valid Product", 99.90m, CategoryId);

        _categoryRepositoryMock
            .Setup(x => x.GetByIdAsync(CategoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Category.NotFound", result.Error.Code);

        _productRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenProductNameIsNotUnique()
    {
        // Arrange
        var command = new CreateProductCommand("Duplicate Product", 99.90m, CategoryId);
        var category = Category.Create("Valid Category").Value;
        var existingProduct = Product.Create("Duplicate Product", 50m, CategoryId).Value;

        _categoryRepositoryMock
            .Setup(x => x.GetByIdAsync(CategoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        _productRepositoryMock
            .Setup(x => x.GetByNameAsync(command.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProduct);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Product.NameNotUnique", result.Error.Code);

        _productRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenDomainValidationFails()
    {
        // Arrange
        var command = new CreateProductCommand("Invalid Price Product", -10m, CategoryId);
        var category = Category.Create("Valid Category").Value;

        _categoryRepositoryMock
            .Setup(x => x.GetByIdAsync(CategoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        _productRepositoryMock
            .Setup(x => x.GetByNameAsync(command.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Product.InvalidPrice", result.Error.Code);

        _productRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
