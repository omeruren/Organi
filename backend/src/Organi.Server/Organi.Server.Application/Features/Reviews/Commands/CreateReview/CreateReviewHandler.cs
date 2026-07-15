using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Reviews.DTOs;
using Organi.Server.Application.Features.Reviews.Mappings;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Enums;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Reviews.Commands.CreateReview;

public sealed class CreateReviewHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser,
    ILogger<CreateReviewHandler> logger) : IRequestHandler<CreateReviewCommand, ReviewResponse>
{
    private static readonly OrderStatus[] QualifyingStatuses =
        [OrderStatus.Confirmed, OrderStatus.Processing, OrderStatus.Shipped, OrderStatus.Delivered];

    public async Task<ReviewResponse> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!.Value;

        var product = await context.Products.FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken)
            ?? throw new NotFoundException("Product", request.ProductId);

        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
            ?? throw new NotFoundException("User", userId);

        var hasQualifyingPurchase = await context.OrderItems
            .AnyAsync(oi => oi.ProductId == request.ProductId
                             && oi.Order.UserId == userId
                             && QualifyingStatuses.Contains(oi.Order.Status), cancellationToken);
        if (!hasQualifyingPurchase)
            throw new BusinessRuleException("You can only review products you have purchased.");

        var alreadyReviewed = await context.Reviews
            .AnyAsync(r => r.ProductId == request.ProductId && r.UserId == userId, cancellationToken);
        if (alreadyReviewed)
            throw new BusinessRuleException("You have already reviewed this product.");

        var review = new Review
        {
            Rating = request.Rating,
            Title = request.Title,
            Comment = request.Comment,
            IsApproved = true,
            ProductId = product.Id,
            Product = product,
            UserId = user.Id,
            User = user
        };

        context.Reviews.Add(review);

        await ReviewAggregateRecalculator.RecalculateAsync(
            context, product, excludeReviewId: null, includeRating: review.Rating, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Review {ReviewId} created for product {ProductId} by user {UserId}", review.Id, product.Id, userId);

        return review.ToResponse();
    }
}
