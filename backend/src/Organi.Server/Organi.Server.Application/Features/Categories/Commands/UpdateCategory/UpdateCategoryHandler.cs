using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Common.Utilities;
using Organi.Server.Application.Features.Categories.DTOs;
using Organi.Server.Application.Features.Categories.Mappings;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Categories.Commands.UpdateCategory;

public sealed class UpdateCategoryHandler(
    IApplicationDbContext context,
    ILogger<UpdateCategoryHandler> logger) : IRequestHandler<UpdateCategoryCommand, CategoryResponse>
{
    private const int MaxNestingDepth = 3;

    public async Task<CategoryResponse> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Category", request.Id);

        if (request.ParentCategoryId.HasValue)
        {
            var newParent = await context.Categories.FirstOrDefaultAsync(c => c.Id == request.ParentCategoryId, cancellationToken)
                ?? throw new NotFoundException("Category", request.ParentCategoryId);

            await EnsureNoCycleAsync(category.Id, newParent, cancellationToken);

            var parentDepth = await ComputeDepthAsync(newParent, cancellationToken);
            if (parentDepth + 1 > MaxNestingDepth)
                throw new BusinessRuleException($"Maximum category nesting depth is {MaxNestingDepth} levels.");
        }

        var nameExists = await context.Categories
            .AnyAsync(c => c.ParentCategoryId == request.ParentCategoryId && c.Name == request.Name && c.Id != category.Id, cancellationToken);
        if (nameExists)
            throw new BusinessRuleException($"A category named '{request.Name}' already exists at this level.");

        if (!string.Equals(request.Name, category.Name, StringComparison.Ordinal))
        {
            category.Slug = await SlugGenerator.GenerateUniqueAsync(
                request.Name,
                candidate => context.Categories.AnyAsync(c => c.Slug == candidate && c.Id != category.Id, cancellationToken));
        }

        category.Name = request.Name;
        category.Description = request.Description;
        category.ImageUrl = request.ImageUrl;
        category.DisplayOrder = request.DisplayOrder;
        category.IsActive = request.IsActive;
        category.ParentCategoryId = request.ParentCategoryId;

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Category {CategoryId} updated", category.Id);

        return category.ToResponse();
    }

    private async Task EnsureNoCycleAsync(Guid categoryId, Category newParent, CancellationToken cancellationToken)
    {
        var current = newParent;

        while (true)
        {
            if (current.Id == categoryId)
                throw new BusinessRuleException("Cannot move a category under one of its own descendants.");

            if (!current.ParentCategoryId.HasValue)
                break;

            current = await context.Categories.FirstAsync(c => c.Id == current.ParentCategoryId, cancellationToken);
        }
    }

    private async Task<int> ComputeDepthAsync(Category category, CancellationToken cancellationToken)
    {
        var depth = 1;
        var current = category;

        while (current.ParentCategoryId.HasValue)
        {
            current = await context.Categories.FirstAsync(c => c.Id == current.ParentCategoryId, cancellationToken);
            depth++;
        }

        return depth;
    }
}
