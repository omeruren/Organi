using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Domain.Enums;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Products.Commands.DeleteProduct;

public sealed class DeleteProductHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser,
    ILogger<DeleteProductHandler> logger) : IRequestHandler<DeleteProductCommand>
{
    private static readonly OrderStatus[] ActiveOrderStatuses =
    [
        OrderStatus.Pending,
        OrderStatus.Confirmed,
        OrderStatus.Processing,
        OrderStatus.Shipped
    ];

    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await context.Products.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Product", request.Id);

        if (!currentUser.IsInRole("Admin") && product.VendorId != currentUser.VendorId)
            throw new ForbiddenException("You do not have permission to delete this product.");

        var hasActiveOrders = await context.OrderItems
            .AnyAsync(oi => oi.ProductId == product.Id && ActiveOrderStatuses.Contains(oi.Order.Status), cancellationToken);

        if (hasActiveOrders)
            throw new BusinessRuleException("Products with pending orders cannot be deleted.");

        context.Products.Remove(product);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Product {ProductId} deleted by user {UserId}", product.Id, currentUser.UserId);
    }
}
