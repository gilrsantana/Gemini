using PersonalFinance.Shared;

namespace PersonalFinance.Domain.Entities;

public class Product : BaseEntity
{
    // Properties
    public string Name { get; private set; }
    public decimal Price { get; private set; }

    // EF Core Constructor
    private Product() : base()
    {
        Name = string.Empty;
    }

    // Full Constructor
    private Product(string name, decimal price) : base()
    {
        Name = name;
        Price = price;
    }

    // Static Factory
    public static Result<Product> Create(string name, decimal price)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure<Product>(new Error("Product.NameRequired", "Product name is required."));
        }

        if (price <= 0)
        {
            return Result.Failure<Product>(new Error("Product.InvalidPrice", "Price must be greater than zero."));
        }

        return new Product(name, price);
    }

    // Mutation Method
    public Result UpdateDetails(string name, decimal price)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure(new Error("Product.NameRequired", "Product name cannot be empty."));
        }

        if (price <= 0)
        {
            return Result.Failure(new Error("Product.InvalidPrice", "Price must be greater than zero."));
        }

        Name = name;
        Price = price;
        
        Update(); // Trigger BaseEntity UpdatedAt update
        
        return Result.Success();
    }
}
