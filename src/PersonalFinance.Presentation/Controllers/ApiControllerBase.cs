using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.Shared;

namespace PersonalFinance.Presentation.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
        {
            return Ok();
        }

        return MapFailureResult(result);
    }

    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return MapFailureResult(result);
    }

    private IActionResult MapFailureResult(Result result)
    {
        var error = result.Error;
        var statusCode = error.Code switch
        {
            "Auth.InvalidCredentials" or 
            "Auth.InvalidToken" or 
            "Token.Expired" or 
            "Auth.InvalidRefreshToken" 
                => StatusCodes.Status401Unauthorized,

            _ when error.Code.Contains("NotFound") 
                => StatusCodes.Status404NotFound,

            _ when error.Code.Contains("Required") || 
            error.Code.Contains("Invalid") || 
            error.Code.Contains("NotUnique") 
                => StatusCodes.Status400BadRequest,
            
            _ => StatusCodes.Status400BadRequest
        };

        var problemDetails = new ProblemDetails
        {
            Title = GetTitleForStatus(statusCode),
            Status = statusCode,
            Detail = error.Message,
            Type = $"https://httpstatuses.com/{statusCode}"
        };

        problemDetails.Extensions.Add("errorCode", error.Code);

        if (error is ValidationError validationError)
        {
            problemDetails.Extensions.Add("propertyName", validationError.PropertyName);
        }

        return StatusCode(statusCode, problemDetails);
    }

    private static string GetTitleForStatus(int statusCode) => statusCode switch
    {
        StatusCodes.Status400BadRequest => "Bad Request",
        StatusCodes.Status401Unauthorized => "Unauthorized",
        StatusCodes.Status404NotFound => "Not Found",
        _ => "An error occurred while processing your request."
    };
}
