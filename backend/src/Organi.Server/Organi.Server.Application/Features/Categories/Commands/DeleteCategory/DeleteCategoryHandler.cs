using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Categories.Commands.DeleteCategory;

public sealed class DeleteCategoryHandler(
    IApplicationDbContext context,
    ILogger<DeleteCategoryHandler> logger) : IRequestHandler<DeleteCategoryCommand>
{
    public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Category", request.Id);

        var hasProducts = await context.Products.AnyAsync(p => p.CategoryId == category.Id, cancellationToken);
        var hasChildren = await context.Categories.AnyAsync(c => c.ParentCategoryId == category.Id, cancellationToken);

        if (hasProducts || hasChildren)
            throw new BusinessRuleException("Categories with products or child categories cannot be deleted.");

        context.Categories.Remove(category);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Category {CategoryId} deleted", category.Id);
    }
}
