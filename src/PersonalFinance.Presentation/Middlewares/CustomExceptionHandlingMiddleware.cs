using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace PersonalFinance.Presentation.Middlewares;

public class CustomExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CustomExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public CustomExceptionHandlingMiddleware(
        RequestDelegate next, 
        ILogger<CustomExceptionHandlingMiddleware> logger, 
        IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An error occurred while processing your request.",
            Detail = _env.IsDevelopment() ? exception.ToString() : exception.Message,
            Type = "https://httpstatuses.com/500"
        };

        var json = JsonSerializer.Serialize(problemDetails);
        await context.Response.WriteAsync(json);
    }
}
