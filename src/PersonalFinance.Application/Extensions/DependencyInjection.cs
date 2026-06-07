using Microsoft.Extensions.DependencyInjection;
using PersonalFinance.Application.Common.CQRS;
using PersonalFinance.Application.UseCases.Products.CommandHandlers;
using PersonalFinance.Application.UseCases.Products.Commands;
using PersonalFinance.Application.UseCases.Products.Queries;
using PersonalFinance.Application.UseCases.Products.QueryHandlers;

namespace PersonalFinance.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Commands
        services.AddScoped<ICommandHandler<CreateProductCommand, Guid>, CreateProductCommandHandler>();

        // Queries
        services.AddScoped<IQueryHandler<GetProductByIdQuery, ProductResponse>, GetProductByIdQueryHandler>();

        return services;
    }
}
