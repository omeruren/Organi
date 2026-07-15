using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Reviews.DTOs;
using Organi.Server.Application.Features.Reviews.Mappings;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Reviews.Commands.UpdateReview;

public sealed class UpdateReviewHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser,
    ILogger<UpdateReviewHandler> logger) : IRequestHandler<UpdateReviewCommand, ReviewResponse>
{
    public async Task<ReviewResponse> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await context.Reviews
            .Include(r => r.Product)
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Review", request.Id);

        if (!currentUser.IsInRole("Admin") && review.UserId != currentUser.UserId)
            throw new ForbiddenException("You do not have permission to modify this review.");

        review.Rating = request.Rating;
        review.Title = request.Title;
        review.Comment = request.Comment;

        await ReviewAggregateRecalculator.RecalculateAsync(
            context, review.Product, excludeReviewId: review.Id, includeRating: review.Rating, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Review {ReviewId} updated by user {UserId}", review.Id, currentUser.UserId);

        return review.ToResponse();
    }
}
