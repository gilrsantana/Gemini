using PersonalFinance.Application.Common.CQRS;
using PersonalFinance.Application.Common.Interfaces;
using PersonalFinance.Application.UseCases.Categories.Commands;
using PersonalFinance.Shared;

namespace PersonalFinance.Application.UseCases.Categories.CommandHandlers;

public class UpdateCategoryCommandHandler : ICommandHandler<UpdateCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(UpdateCategoryCommand command, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(command.Id, cancellationToken);
        if (category is null)
        {
            return Result.Failure(new Error("Category.NotFound", "Category not found."));
        }

        if (category.Name != command.Name)
        {
            var existingCategory = await _categoryRepository.GetByNameAsync(command.Name, cancellationToken);
            if (existingCategory is not null)
            {
                return Result.Failure(new Error("Category.NameNotUnique", "Category name must be unique."));
            }
        }

        var updateResult = category.UpdateDetails(command.Name);
        if (updateResult.IsFailure)
        {
            return Result.Failure(updateResult.Error);
        }

        _categoryRepository.Update(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
