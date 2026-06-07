using PersonalFinance.Application.Common.CQRS;

namespace PersonalFinance.Application.UseCases.Products.Commands;

public record CreateProductCommand(string Name, decimal Price) : ICommand<Guid>;
