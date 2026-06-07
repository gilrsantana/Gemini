using PersonalFinance.Application.Common.CQRS;

namespace PersonalFinance.Application.UseCases.Categories.Commands;

public record CreateCategoryCommand(string Name) : ICommand<Guid>;
