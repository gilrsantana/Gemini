using PersonalFinance.Application.Common.CQRS;

namespace PersonalFinance.Application.UseCases.Categories.Queries;

public record GetCategoryByIdQuery(Guid CategoryId) : IQuery<CategoryResponse>;
