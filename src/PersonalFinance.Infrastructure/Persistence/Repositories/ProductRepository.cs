using Microsoft.EntityFrameworkCore;
using PersonalFinance.Application.Common.Interfaces;
using PersonalFinance.Domain.Entities;

namespace PersonalFinance.Infrastructure.Persistence.Repositories;

public class ProductRepository : BaseEntityRepository<Product>, IProductRepository
{
    public ProductRepository(PersonalFinanceDbContext context) : base(context)
    {
    }

    public async Task<Product?> GetByNameAsync(string name, CancellationToken cancellationToken = default) =>
        await DbSet.FirstOrDefaultAsync(p => p.Name == name, cancellationToken);
}
