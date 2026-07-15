using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Compare.DTOs;
using Organi.Server.Application.Features.Compare.Mappings;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Compare.Commands.AddCompareItem;

public sealed class AddCompareItemHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser,
    ILogger<AddCompareItemHandler> logger) : IRequestHandler<AddCompareItemCommand, CompareItemResponse>
{
    public async Task<CompareItemResponse> Handle(AddCompareItemCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!.Value;

        var product = await context.Products
            .Include(p => p.ProductImages)
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken)
            ?? throw new NotFoundException("Product", request.ProductId);

        var existingItem = await context.CompareItems
            .Include(c => c.Product)
            .ThenInclude(p => p.ProductImages)
            .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == request.ProductId, cancellationToken);

        if (existingItem is not null)
            return existingItem.ToResponse();

        var item = new CompareItem
        {
            UserId = userId,
            ProductId = product.Id,
            Product = product
        };

        context.CompareItems.Add(item);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Product {ProductId} added to compare list for user {UserId}", product.Id, userId);

        return item.ToResponse();
    }
}
