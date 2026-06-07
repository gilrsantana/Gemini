using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.Application.Common.CQRS;
using PersonalFinance.Application.UseCases.Products.Commands;
using PersonalFinance.Application.UseCases.Products.Queries;

namespace PersonalFinance.Presentation.Controllers;

public class ProductsController : ApiControllerBase
{
    private readonly ICommandHandler<CreateProductCommand, Guid> _createHandler;
    private readonly IQueryHandler<GetProductByIdQuery, ProductResponse> _getByIdHandler;

    public ProductsController(
        ICommandHandler<CreateProductCommand, Guid> createHandler,
        IQueryHandler<GetProductByIdQuery, ProductResponse> getByIdHandler)
    {
        _createHandler = createHandler;
        _getByIdHandler = getByIdHandler;
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetProductByIdQuery(id);
        var result = await _getByIdHandler.HandleAsync(query, cancellationToken);
        
        return HandleResult(result);
    }

    [HttpPost]
    [AllowAnonymous] // Allowing anonymous creation for ease of testing in this boilerplate
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command, CancellationToken cancellationToken)
    {
        var result = await _createHandler.HandleAsync(command, cancellationToken);
        
        if (result.IsFailure)
        {
            return HandleResult(result);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }
}
