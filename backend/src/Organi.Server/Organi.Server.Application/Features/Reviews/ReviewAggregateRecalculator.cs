using Microsoft.EntityFrameworkCore;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Domain.Entities;

namespace Organi.Server.Application.Features.Reviews;

public static class ReviewAggregateRecalculator
{
    /// <summary>
    /// Recomputes Product.AverageRating/ReviewCount from approved reviews already persisted in the
    /// database, adjusted in-memory for the review currently being written — this avoids requiring
    /// an extra SaveChangesAsync round-trip just to make an in-flight Add/Update visible to the query.
    /// </summary>
    /// <param name="excludeReviewId">A review Id to exclude from the database read (the review being updated or deleted).</param>
    /// <param name="includeRating">A rating to add on top of the database read (the new/updated review's rating).</param>
    public static async Task RecalculateAsync(
        IApplicationDbContext context,
        Product product,
        Guid? excludeReviewId,
        int? includeRating,
        CancellationToken cancellationToken)
    {
        var ratings = await context.Reviews
            .Where(r => r.ProductId == product.Id && r.IsApproved && r.Id != excludeReviewId)
            .Select(r => r.Rating)
            .ToListAsync(cancellationToken);

        if (includeRating.HasValue)
            ratings.Add(includeRating.Value);

        product.AverageRating = ratings.Count > 0 ? (decimal)ratings.Average() : 0m;
        product.ReviewCount = ratings.Count;
    }
}
