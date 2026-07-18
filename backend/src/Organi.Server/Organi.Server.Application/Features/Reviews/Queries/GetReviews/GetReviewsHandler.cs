using MediatR;
using Microsoft.EntityFrameworkCore;
using Organi.Server.Application.Common.Extensions;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Common.Models;
using Organi.Server.Application.Features.Reviews.DTOs;

namespace Organi.Server.Application.Features.Reviews.Queries.GetReviews;

public sealed class GetReviewsHandler(
    IApplicationDbContext context) : IRequestHandler<GetReviewsQuery, PagedResponse<AdminReviewResponse>>
{
    public async Task<PagedResponse<AdminReviewResponse>> Handle(GetReviewsQuery request, CancellationToken cancellationToken)
    {
        // Admin moderation view — deliberately no IsApproved filter; moderators must see everything.
        var query = context.Reviews.AsNoTracking();

        if (request.ProductId.HasValue)
            query = query.Where(r => r.ProductId == request.ProductId.Value);

        if (request.Rating.HasValue)
            query = query.Where(r => r.Rating == request.Rating.Value);

        var projected = query
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new AdminReviewResponse(
                r.Id,
                r.Rating,
                r.Title,
                r.Comment,
                r.ProductId,
                r.Product.Name,
                r.UserId,
                r.User.FirstName + " " + r.User.LastName,
                r.CreatedAt,
                r.UpdatedAt));

        return await projected.ToPagedResponseAsync(request.Page, request.PageSize, cancellationToken);
    }
}
