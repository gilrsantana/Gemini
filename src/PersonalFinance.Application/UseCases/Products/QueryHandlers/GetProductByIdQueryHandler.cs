using PersonalFinance.Application.Common.CQRS;
using PersonalFinance.Application.Common.Interfaces;
using PersonalFinance.Application.UseCases.Products.Queries;
using PersonalFinance.Shared;

namespace PersonalFinance.Application.UseCases.Products.QueryHandlers;

public class GetProductByIdQueryHandler : IQueryHandler<GetProductByIdQuery, ProductResponse>
{
    private readonly IProductRepository _productRepository;

    public GetProductByIdQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<ProductResponse>> HandleAsync(GetProductByIdQuery query, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(query.ProductId, cancellationToken);
        if (product is null)
        {
            return Result.Failure<ProductResponse>(new Error("Product.NotFound", "Product not found."));
        }

        var response = new ProductResponse(product.Id, product.Name, product.Price, product.CreatedAt);
        return response;
    }
}
