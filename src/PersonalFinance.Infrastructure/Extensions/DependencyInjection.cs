using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PersonalFinance.Application.Common.Interfaces;
using PersonalFinance.Infrastructure.Configurations;
using PersonalFinance.Infrastructure.Identity;
using PersonalFinance.Infrastructure.Persistence;
using PersonalFinance.Infrastructure.Persistence.Repositories;

namespace PersonalFinance.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));
        var dbOptions = configuration.GetSection(DatabaseOptions.SectionName).Get<DatabaseOptions>() ?? new DatabaseOptions();

        services.AddDbContext<PersonalFinanceDbContext>(options =>
        {
            options.UseMySQL(connectionString, mysqlOptions =>
            {
                mysqlOptions.CommandTimeout(dbOptions.CommandTimeout);
                if (dbOptions.MaxBatchSize.HasValue)
                {
                    mysqlOptions.MaxBatchSize(dbOptions.MaxBatchSize.Value);
                }
            });

            if (dbOptions.EnableDetailedErrors)
            {
                options.EnableDetailedErrors();
            }
            if (dbOptions.EnableSensitiveDataLogging)
            {
                options.EnableSensitiveDataLogging();
            }
        });

        // Identity Service
        services.AddScoped<IIdentityService, IdentityService>();

        // Repositories & Unit of Work
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<IProductRepository>());

        return services;
    }
}
