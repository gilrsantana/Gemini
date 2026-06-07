using PersonalFinance.Domain.Entities;

namespace PersonalFinance.UnitTests.Domain;

public class CategoryTests
{
    [Fact]
    public void Create_ShouldReturnSuccess_WhenArgumentsAreValid()
    {
        // Act
        var result = Category.Create("Electronics");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Electronics", result.Value.Name);
        Assert.NotEqual(Guid.Empty, result.Value.Id);
        Assert.True(result.Value.Active);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenNameIsEmpty()
    {
        // Act
        var result1 = Category.Create("");
        var result2 = Category.Create("   ");

        // Assert
        Assert.True(result1.IsFailure);
        Assert.Equal("Category.NameRequired", result1.Error.Code);

        Assert.True(result2.IsFailure);
        Assert.Equal("Category.NameRequired", result2.Error.Code);
    }

    [Fact]
    public void UpdateDetails_ShouldUpdatePropertiesAndTriggerUpdate_WhenArgumentsAreValid()
    {
        // Arrange
        var category = Category.Create("Original").Value;
        var originalUpdatedAt = category.UpdatedAt;

        // Act
        var result = category.UpdateDetails("Updated");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Updated", category.Name);
        Assert.NotNull(category.UpdatedAt);
        Assert.NotEqual(originalUpdatedAt, category.UpdatedAt);
    }

    [Fact]
    public void UpdateDetails_ShouldReturnFailure_WhenNameIsEmpty()
    {
        // Arrange
        var category = Category.Create("Original").Value;

        // Act
        var result = category.UpdateDetails("");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Category.NameRequired", result.Error.Code);
    }
}
