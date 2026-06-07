using Microsoft.EntityFrameworkCore;
using PersonalFinance.Domain.Entities;

namespace PersonalFinance.Infrastructure.Persistence;

public class PersonalFinanceDbContext : DbContext
{
    public PersonalFinanceDbContext(DbContextOptions<PersonalFinanceDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(PersonalFinanceDbContext).Assembly);
    }
}
