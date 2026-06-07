using PersonalFinance.Shared;

namespace PersonalFinance.Domain.Entities;

public class Category : BaseEntity
{
    // Properties
    public string Name { get; private set; }

    // Relationship
    private readonly List<Product> _products = new();
    public virtual IReadOnlyCollection<Product> Products => _products.AsReadOnly();

    // EF Core Constructor
    private Category() : base()
    {
        Name = string.Empty;
    }

    // Full Constructor
    private Category(string name) : base()
    {
        Name = name;
    }

    // Static Factory
    public static Result<Category> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure<Category>(new Error("Category.NameRequired", "Category name is required."));
        }

        return new Category(name);
    }

    // Mutation Method
    public Result UpdateDetails(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure(new Error("Category.NameRequired", "Category name cannot be empty."));
        }

        Name = name;
        
        Update(); // Trigger BaseEntity UpdatedAt update
        
        return Result.Success();
    }
}
