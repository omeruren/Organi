using MediatR;
using Microsoft.EntityFrameworkCore;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Categories.DTOs;
using Organi.Server.Application.Features.Categories.Mappings;
using Organi.Server.Domain.Entities;

namespace Organi.Server.Application.Features.Categories.Queries.GetCategories;

public sealed class GetCategoriesHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser) : IRequestHandler<GetCategoriesQuery, IReadOnlyList<CategoryResponse>>
{
    public async Task<IReadOnlyList<CategoryResponse>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var isAdmin = currentUser.IsInRole("Admin");

        var categories = await context.Categories
            .AsNoTracking()
            .Where(c => isAdmin || c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync(cancellationToken);

        var byId = categories.ToDictionary(c => c.Id);
        var roots = new List<Category>();

        foreach (var category in categories)
        {
            if (category.ParentCategoryId.HasValue && byId.TryGetValue(category.ParentCategoryId.Value, out var parent))
                parent.ChildCategories.Add(category);
            else
                roots.Add(category);
        }

        return roots.Select(c => c.ToResponse()).ToList();
    }
}
