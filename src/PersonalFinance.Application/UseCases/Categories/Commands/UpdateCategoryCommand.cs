using PersonalFinance.Application.Common.CQRS;

namespace PersonalFinance.Application.UseCases.Categories.Commands;

public record UpdateCategoryCommand(Guid Id, string Name) : ICommand;
