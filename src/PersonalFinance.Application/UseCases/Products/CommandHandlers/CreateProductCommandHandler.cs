using PersonalFinance.Application.Common.CQRS;
using PersonalFinance.Application.Common.Interfaces;
using PersonalFinance.Application.UseCases.Products.Commands;
using PersonalFinance.Domain.Entities;
using PersonalFinance.Shared;

namespace PersonalFinance.Application.UseCases.Products.CommandHandlers;

public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, Guid>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> HandleAsync(CreateProductCommand command, CancellationToken cancellationToken = default)
    {
        var existingProduct = await _productRepository.GetByNameAsync(command.Name, cancellationToken);
        if (existingProduct is not null)
        {
            return Result.Failure<Guid>(new Error("Product.NameNotUnique", "Product name must be unique."));
        }

        var productResult = Product.Create(command.Name, command.Price);
        if (productResult.IsFailure)
        {
            return Result.Failure<Guid>(productResult.Error);
        }

        await _productRepository.AddAsync(productResult.Value, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return productResult.Value.Id;
    }
}
