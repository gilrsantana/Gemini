using PersonalFinance.Application.Common.CQRS;
using PersonalFinance.Application.Common.Interfaces;
using PersonalFinance.Application.UseCases.Categories.Commands;
using PersonalFinance.Domain.Entities;
using PersonalFinance.Shared;

namespace PersonalFinance.Application.UseCases.Categories.CommandHandlers;

public class CreateCategoryCommandHandler : ICommandHandler<CreateCategoryCommand, Guid>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> HandleAsync(CreateCategoryCommand command, CancellationToken cancellationToken = default)
    {
        var existingCategory = await _categoryRepository.GetByNameAsync(command.Name, cancellationToken);
        if (existingCategory is not null)
        {
            return Result.Failure<Guid>(new Error("Category.NameNotUnique", "Category name must be unique."));
        }

        var categoryResult = Category.Create(command.Name);
        if (categoryResult.IsFailure)
        {
            return Result.Failure<Guid>(categoryResult.Error);
        }

        await _categoryRepository.AddAsync(categoryResult.Value, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return categoryResult.Value.Id;
    }
}
