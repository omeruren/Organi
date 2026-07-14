using MediatR;
using Organi.Server.Application.Features.Categories.DTOs;

namespace Organi.Server.Application.Features.Categories.Commands.CreateCategory;

public sealed record CreateCategoryCommand(
    string Name,
    string? Description,
    string? ImageUrl,
    int DisplayOrder,
    Guid? ParentCategoryId) : IRequest<CategoryResponse>;
