using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.Application.Common.CQRS;
using PersonalFinance.Application.UseCases.Categories.Commands;
using PersonalFinance.Application.UseCases.Categories.Queries;

namespace PersonalFinance.Presentation.Controllers;

public class CategoriesController : ApiControllerBase
{
    private readonly ICommandHandler<CreateCategoryCommand, Guid> _createHandler;
    private readonly ICommandHandler<UpdateCategoryCommand> _updateHandler;
    private readonly IQueryHandler<GetCategoryByIdQuery, CategoryResponse> _getByIdHandler;

    public CategoriesController(
        ICommandHandler<CreateCategoryCommand, Guid> createHandler,
        ICommandHandler<UpdateCategoryCommand> updateHandler,
        IQueryHandler<GetCategoryByIdQuery, CategoryResponse> getByIdHandler)
    {
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _getByIdHandler = getByIdHandler;
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetCategoryByIdQuery(id);
        var result = await _getByIdHandler.HandleAsync(query, cancellationToken);
        
        return HandleResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        var result = await _createHandler.HandleAsync(command, cancellationToken);
        
        if (result.IsFailure)
        {
            return HandleResult(result);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Routing ID does not match Command ID."
            });
        }

        var result = await _updateHandler.HandleAsync(command, cancellationToken);
        
        return HandleResult(result);
    }
}
