using Microsoft.OpenApi;
using PersonalFinance.Application.Extensions;
using PersonalFinance.Infrastructure.Extensions;
using PersonalFinance.Presentation.Middlewares;
using Scalar.AspNetCore;

namespace PersonalFinance.Presentation.Configurations;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();

        // CORS configuration
        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });

        // Register authentication/authorization services (simple/no-op by default)
        services.AddAuthentication();
        services.AddAuthorization();

        // OpenAPI & Scalar configuration
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                var scheme = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "Input your JWT Bearer token to access protected endpoints."
                };

                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
                document.Components.SecuritySchemes.Add("Bearer", scheme);

                var requirement = new OpenApiSecurityRequirement
                {
                    { new OpenApiSecuritySchemeReference("Bearer", document), new List<string>() }
                };
                
                document.Security ??= new List<OpenApiSecurityRequirement>();
                document.Security.Add(requirement);

                return Task.CompletedTask;
            });
        });

        // Chain registrations
        services.AddApplication()
                .AddInfrastructure(configuration);

        return services;
    }

    public static void Configure(this WebApplication app)
    {
        // Must be the very first in the pipeline
        app.UseMiddleware<CustomExceptionHandlingMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference(options =>
            {
                options.WithTitle("PersonalFinance Web API")
                       .WithTheme(ScalarTheme.Moon)
                       .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
            });
        }

        app.UseHttpsRedirection();
        app.UseCors("AllowFrontend");
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
    }
}
