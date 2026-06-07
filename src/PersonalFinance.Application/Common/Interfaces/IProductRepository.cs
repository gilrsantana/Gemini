using PersonalFinance.Domain.Entities;

namespace PersonalFinance.Application.Common.Interfaces;

public interface IProductRepository : IUnitOfWork
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Product?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    void Update(Product product);
}
