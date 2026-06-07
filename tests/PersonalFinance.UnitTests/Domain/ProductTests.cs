using PersonalFinance.Domain.Entities;
using Xunit;

namespace PersonalFinance.UnitTests.Domain;

public class ProductTests
{
    [Fact]
    public void Create_ShouldReturnSuccess_WhenArgumentsAreValid()
    {
        // Act
        var result = Product.Create("Test Product", 99.90m);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Test Product", result.Value.Name);
        Assert.Equal(99.90m, result.Value.Price);
        Assert.NotEqual(Guid.Empty, result.Value.Id);
        Assert.True(result.Value.Active);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenNameIsEmpty()
    {
        // Act
        var result = Product.Create("", 99.90m);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Product.NameRequired", result.Error.Code);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenPriceIsZeroOrNegative()
    {
        // Act
        var result1 = Product.Create("Test Product", 0);
        var result2 = Product.Create("Test Product", -10m);

        // Assert
        Assert.True(result1.IsFailure);
        Assert.Equal("Product.InvalidPrice", result1.Error.Code);

        Assert.True(result2.IsFailure);
        Assert.Equal("Product.InvalidPrice", result2.Error.Code);
    }

    [Fact]
    public void UpdateDetails_ShouldUpdatePropertiesAndTriggerUpdate_WhenArgumentsAreValid()
    {
        // Arrange
        var product = Product.Create("Original", 10m).Value;
        var originalUpdatedAt = product.UpdatedAt;

        // Act
        var result = product.UpdateDetails("Updated", 20m);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Updated", product.Name);
        Assert.Equal(20m, product.Price);
        Assert.NotNull(product.UpdatedAt);
        Assert.NotEqual(originalUpdatedAt, product.UpdatedAt);
    }

    [Fact]
    public void UpdateDetails_ShouldReturnFailure_WhenNameIsEmpty()
    {
        // Arrange
        var product = Product.Create("Original", 10m).Value;

        // Act
        var result = product.UpdateDetails("", 20m);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Product.NameRequired", result.Error.Code);
    }

    [Fact]
    public void UpdateDetails_ShouldReturnFailure_WhenPriceIsZeroOrNegative()
    {
        // Arrange
        var product = Product.Create("Original", 10m).Value;

        // Act
        var result = product.UpdateDetails("Updated", -5m);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Product.InvalidPrice", result.Error.Code);
    }
}
