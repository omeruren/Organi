using MediatR;
using Organi.Server.Application.Features.Categories.DTOs;

namespace Organi.Server.Application.Features.Categories.Commands.UpdateCategory;

public sealed record UpdateCategoryCommand(
    string Name,
    string? Description,
    string? ImageUrl,
    int DisplayOrder,
    bool IsActive,
    Guid? ParentCategoryId) : IRequest<CategoryResponse>
{
    public Guid Id { get; init; }
}
