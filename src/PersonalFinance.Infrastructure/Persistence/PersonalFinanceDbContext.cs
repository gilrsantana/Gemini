using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PersonalFinance.Domain.Entities;
using PersonalFinance.Infrastructure.Identity;

namespace PersonalFinance.Infrastructure.Persistence;

public class PersonalFinanceDbContext : IdentityDbContext<Account, Role, Guid>
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
        
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("AccountClaims");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("AccountLogins");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("AccountTokens");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");

        builder.ApplyConfigurationsFromAssembly(typeof(PersonalFinanceDbContext).Assembly);
    }
}
