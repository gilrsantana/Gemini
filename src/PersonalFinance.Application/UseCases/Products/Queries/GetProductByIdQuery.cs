using PersonalFinance.Application.Common.CQRS;

namespace PersonalFinance.Application.UseCases.Products.Queries;

public record GetProductByIdQuery(Guid ProductId) : IQuery<ProductResponse>;
