using PersonalFinance.Application.Common.CQRS;
using PersonalFinance.Application.Common.Interfaces;
using PersonalFinance.Application.UseCases.Products.Commands;
using PersonalFinance.Domain.Entities;
using PersonalFinance.Shared;

namespace PersonalFinance.Application.UseCases.Products.CommandHandlers;

public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, Guid>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> HandleAsync(CreateProductCommand command, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(command.CategoryId, cancellationToken);
        if (category is null)
        {
            return Result.Failure<Guid>(new Error("Category.NotFound", "The specified category was not found."));
        }

        var existingProduct = await _productRepository.GetByNameAsync(command.Name, cancellationToken);
        if (existingProduct is not null)
        {
            return Result.Failure<Guid>(new Error("Product.NameNotUnique", "Product name must be unique."));
        }

        var productResult = Product.Create(command.Name, command.Price, command.CategoryId);
        if (productResult.IsFailure)
        {
            return Result.Failure<Guid>(productResult.Error);
        }

        await _productRepository.AddAsync(productResult.Value, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return productResult.Value.Id;
    }
}
