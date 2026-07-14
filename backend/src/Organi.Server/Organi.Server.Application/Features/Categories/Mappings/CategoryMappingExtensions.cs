using Organi.Server.Application.Features.Categories.DTOs;
using Organi.Server.Domain.Entities;

namespace Organi.Server.Application.Features.Categories.Mappings;

public static class CategoryMappingExtensions
{
    public static CategoryResponse ToResponse(this Category category) => new(
        category.Id,
        category.Name,
        category.Slug,
        category.Description,
        category.ImageUrl,
        category.DisplayOrder,
        category.IsActive,
        category.ParentCategoryId,
        category.ChildCategories
            .OrderBy(c => c.DisplayOrder)
            .Select(c => c.ToResponse())
            .ToList());
}
