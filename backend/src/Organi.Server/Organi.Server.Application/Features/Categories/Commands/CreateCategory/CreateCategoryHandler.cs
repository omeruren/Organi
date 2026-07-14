using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Common.Utilities;
using Organi.Server.Application.Features.Categories.DTOs;
using Organi.Server.Application.Features.Categories.Mappings;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Categories.Commands.CreateCategory;

public sealed class CreateCategoryHandler(
    IApplicationDbContext context,
    ILogger<CreateCategoryHandler> logger) : IRequestHandler<CreateCategoryCommand, CategoryResponse>
{
    private const int MaxNestingDepth = 3;

    public async Task<CategoryResponse> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        if (request.ParentCategoryId.HasValue)
        {
            var parent = await context.Categories.FirstOrDefaultAsync(c => c.Id == request.ParentCategoryId, cancellationToken)
                ?? throw new NotFoundException("Category", request.ParentCategoryId);

            var parentDepth = await ComputeDepthAsync(parent, cancellationToken);
            if (parentDepth + 1 > MaxNestingDepth)
                throw new BusinessRuleException($"Maximum category nesting depth is {MaxNestingDepth} levels.");
        }

        var nameExists = await context.Categories
            .AnyAsync(c => c.ParentCategoryId == request.ParentCategoryId && c.Name == request.Name, cancellationToken);
        if (nameExists)
            throw new BusinessRuleException($"A category named '{request.Name}' already exists at this level.");

        var slug = await SlugGenerator.GenerateUniqueAsync(
            request.Name,
            candidate => context.Categories.AnyAsync(c => c.Slug == candidate, cancellationToken));

        var category = new Category
        {
            Name = request.Name,
            Slug = slug,
            Description = request.Description,
            ImageUrl = request.ImageUrl,
            DisplayOrder = request.DisplayOrder,
            IsActive = true,
            ParentCategoryId = request.ParentCategoryId
        };

        context.Categories.Add(category);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Category {CategoryId} created", category.Id);

        return category.ToResponse();
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
