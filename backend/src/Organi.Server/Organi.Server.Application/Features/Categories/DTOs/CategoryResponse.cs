namespace Organi.Server.Application.Features.Categories.DTOs;

public sealed record CategoryResponse(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    string? ImageUrl,
    int DisplayOrder,
    bool IsActive,
    Guid? ParentCategoryId,
    IReadOnlyList<CategoryResponse> Children);
