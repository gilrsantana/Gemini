using Microsoft.Extensions.DependencyInjection;
using PersonalFinance.Application.Common.CQRS;
using PersonalFinance.Application.UseCases.Products.CommandHandlers;
using PersonalFinance.Application.UseCases.Products.Commands;
using PersonalFinance.Application.UseCases.Products.Queries;
using PersonalFinance.Application.UseCases.Products.QueryHandlers;
using PersonalFinance.Application.UseCases.Categories.CommandHandlers;
using PersonalFinance.Application.UseCases.Categories.Commands;
using PersonalFinance.Application.UseCases.Categories.Queries;
using PersonalFinance.Application.UseCases.Categories.QueryHandlers;

namespace PersonalFinance.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Commands
        services.AddScoped<ICommandHandler<CreateProductCommand, Guid>, CreateProductCommandHandler>();
        services.AddScoped<ICommandHandler<CreateCategoryCommand, Guid>, CreateCategoryCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateCategoryCommand>, UpdateCategoryCommandHandler>();

        // Queries
        services.AddScoped<IQueryHandler<GetProductByIdQuery, ProductResponse>, GetProductByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetCategoryByIdQuery, CategoryResponse>, GetCategoryByIdQueryHandler>();

        return services;
    }
}
