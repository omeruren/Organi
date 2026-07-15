using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Compare.Commands.RemoveCompareItem;

public sealed class RemoveCompareItemHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser,
    ILogger<RemoveCompareItemHandler> logger) : IRequestHandler<RemoveCompareItemCommand>
{
    public async Task Handle(RemoveCompareItemCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!.Value;

        var item = await context.CompareItems
            .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == request.ProductId, cancellationToken)
            ?? throw new NotFoundException("CompareItem", request.ProductId);

        context.CompareItems.Remove(item);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Product {ProductId} removed from compare list for user {UserId}", request.ProductId, userId);
    }
}
