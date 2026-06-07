namespace PersonalFinance.Application.UseCases.Products.Queries;

public record ProductResponse(Guid Id, string Name, decimal Price, Guid CategoryId, DateTime CreatedAt);
