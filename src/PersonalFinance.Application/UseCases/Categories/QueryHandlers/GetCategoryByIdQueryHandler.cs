using PersonalFinance.Application.Common.CQRS;
using PersonalFinance.Application.Common.Interfaces;
using PersonalFinance.Application.UseCases.Categories.Queries;
using PersonalFinance.Shared;

namespace PersonalFinance.Application.UseCases.Categories.QueryHandlers;

public class GetCategoryByIdQueryHandler : IQueryHandler<GetCategoryByIdQuery, CategoryResponse>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<CategoryResponse>> HandleAsync(GetCategoryByIdQuery query, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(query.CategoryId, cancellationToken);
        if (category is null)
        {
            return Result.Failure<CategoryResponse>(new Error("Category.NotFound", "Category not found."));
        }

        var response = new CategoryResponse(category.Id, category.Name, category.CreatedAt);
        return response;
    }
}
