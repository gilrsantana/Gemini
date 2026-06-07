using Microsoft.EntityFrameworkCore;
using PersonalFinance.Application.Common.Interfaces;
using PersonalFinance.Domain.Entities;

namespace PersonalFinance.Infrastructure.Persistence.Repositories;

public class CategoryRepository : BaseEntityRepository<Category>, ICategoryRepository
{
    public CategoryRepository(PersonalFinanceDbContext context) : base(context)
    {
    }

    public async Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default) =>
        await DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower(), cancellationToken);
}
