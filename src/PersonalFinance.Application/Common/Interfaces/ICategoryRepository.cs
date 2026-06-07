using PersonalFinance.Domain.Entities;

namespace PersonalFinance.Application.Common.Interfaces;

public interface ICategoryRepository : IUnitOfWork
{
    Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task AddAsync(Category category, CancellationToken cancellationToken = default);
    void Update(Category category);
}
