using PersonalFinance.Shared;

namespace PersonalFinance.Domain.Entities;

public class Product : BaseEntity
{
    // Properties
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public Guid CategoryId { get; private set; }
    public virtual Category Category { get; private set; } = null!;

    // EF Core Constructor
    private Product() : base()
    {
        Name = string.Empty;
    }

    // Full Constructor
    private Product(string name, decimal price, Guid categoryId) : base()
    {
        Name = name;
        Price = price;
        CategoryId = categoryId;
    }

    // Static Factory
    public static Result<Product> Create(string name, decimal price, Guid categoryId)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure<Product>(new Error("Product.NameRequired", "Product name is required."));
        }

        if (price <= 0)
        {
            return Result.Failure<Product>(new Error("Product.InvalidPrice", "Price must be greater than zero."));
        }

        if (categoryId == Guid.Empty)
        {
            return Result.Failure<Product>(new Error("Product.CategoryIdRequired", "Category ID is required."));
        }

        return new Product(name, price, categoryId);
    }

    // Mutation Method
    public Result UpdateDetails(string name, decimal price, Guid categoryId)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure(new Error("Product.NameRequired", "Product name cannot be empty."));
        }

        if (price <= 0)
        {
            return Result.Failure(new Error("Product.InvalidPrice", "Price must be greater than zero."));
        }

        if (categoryId == Guid.Empty)
        {
            return Result.Failure(new Error("Product.CategoryIdRequired", "Category ID is required."));
        }

        Name = name;
        Price = price;
        CategoryId = categoryId;
        
        Update(); // Trigger BaseEntity UpdatedAt update
        
        return Result.Success();
    }
}
