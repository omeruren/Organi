using Organi.Server.Application.Features.Reviews.DTOs;
using Organi.Server.Domain.Entities;

namespace Organi.Server.Application.Features.Reviews.Mappings;

public static class ReviewMappingExtensions
{
    public static ReviewResponse ToResponse(this Review review) => new(
        review.Id,
        review.Rating,
        review.Title,
        review.Comment,
        review.ProductId,
        review.UserId,
        $"{review.User.FirstName} {review.User.LastName}",
        review.CreatedAt,
        review.UpdatedAt);
}
