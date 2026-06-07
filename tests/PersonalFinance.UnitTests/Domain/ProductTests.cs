using PersonalFinance.Domain.Entities;

namespace PersonalFinance.UnitTests.Domain;

public class ProductTests
{
    private static readonly Guid ValidCategoryId = Guid.NewGuid();

    [Fact]
    public void Create_ShouldReturnSuccess_WhenArgumentsAreValid()
    {
        // Act
        var result = Product.Create("Test Product", 99.90m, ValidCategoryId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Test Product", result.Value.Name);
        Assert.Equal(99.90m, result.Value.Price);
        Assert.Equal(ValidCategoryId, result.Value.CategoryId);
        Assert.NotEqual(Guid.Empty, result.Value.Id);
        Assert.True(result.Value.Active);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenNameIsEmpty()
    {
        // Act
        var result = Product.Create("", 99.90m, ValidCategoryId);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Product.NameRequired", result.Error.Code);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenPriceIsZeroOrNegative()
    {
        // Act
        var result1 = Product.Create("Test Product", 0, ValidCategoryId);
        var result2 = Product.Create("Test Product", -10m, ValidCategoryId);

        // Assert
        Assert.True(result1.IsFailure);
        Assert.Equal("Product.InvalidPrice", result1.Error.Code);

        Assert.True(result2.IsFailure);
        Assert.Equal("Product.InvalidPrice", result2.Error.Code);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenCategoryIdIsEmpty()
    {
        // Act
        var result = Product.Create("Test Product", 99.90m, Guid.Empty);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Product.CategoryIdRequired", result.Error.Code);
    }

    [Fact]
    public void UpdateDetails_ShouldUpdatePropertiesAndTriggerUpdate_WhenArgumentsAreValid()
    {
        // Arrange
        var product = Product.Create("Original", 10m, ValidCategoryId).Value;
        var originalUpdatedAt = product.UpdatedAt;
        var newCategoryId = Guid.NewGuid();

        // Act
        var result = product.UpdateDetails("Updated", 20m, newCategoryId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Updated", product.Name);
        Assert.Equal(20m, product.Price);
        Assert.Equal(newCategoryId, product.CategoryId);
        Assert.NotNull(product.UpdatedAt);
        Assert.NotEqual(originalUpdatedAt, product.UpdatedAt);
    }

    [Fact]
    public void UpdateDetails_ShouldReturnFailure_WhenNameIsEmpty()
    {
        // Arrange
        var product = Product.Create("Original", 10m, ValidCategoryId).Value;

        // Act
        var result = product.UpdateDetails("", 20m, ValidCategoryId);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Product.NameRequired", result.Error.Code);
    }

    [Fact]
    public void UpdateDetails_ShouldReturnFailure_WhenPriceIsZeroOrNegative()
    {
        // Arrange
        var product = Product.Create("Original", 10m, ValidCategoryId).Value;

        // Act
        var result = product.UpdateDetails("Updated", -5m, ValidCategoryId);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Product.InvalidPrice", result.Error.Code);
    }

    [Fact]
    public void UpdateDetails_ShouldReturnFailure_WhenCategoryIdIsEmpty()
    {
        // Arrange
        var product = Product.Create("Original", 10m, ValidCategoryId).Value;

        // Act
        var result = product.UpdateDetails("Updated", 20m, Guid.Empty);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Product.CategoryIdRequired", result.Error.Code);
    }
}
