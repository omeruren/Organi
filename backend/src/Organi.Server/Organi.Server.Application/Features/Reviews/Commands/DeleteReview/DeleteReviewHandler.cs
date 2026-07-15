using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Reviews.Commands.DeleteReview;

public sealed class DeleteReviewHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser,
    ILogger<DeleteReviewHandler> logger) : IRequestHandler<DeleteReviewCommand>
{
    public async Task Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await context.Reviews
            .Include(r => r.Product)
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Review", request.Id);

        if (!currentUser.IsInRole("Admin") && review.UserId != currentUser.UserId)
            throw new ForbiddenException("You do not have permission to delete this review.");

        context.Reviews.Remove(review);

        await ReviewAggregateRecalculator.RecalculateAsync(
            context, review.Product, excludeReviewId: review.Id, includeRating: null, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Review {ReviewId} deleted by user {UserId}", review.Id, currentUser.UserId);
    }
}
