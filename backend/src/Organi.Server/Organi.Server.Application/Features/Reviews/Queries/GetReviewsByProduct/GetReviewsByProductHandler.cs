using MediatR;
using Microsoft.EntityFrameworkCore;
using Organi.Server.Application.Common.Extensions;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Common.Models;
using Organi.Server.Application.Features.Reviews.DTOs;

namespace Organi.Server.Application.Features.Reviews.Queries.GetReviewsByProduct;

public sealed class GetReviewsByProductHandler(
    IApplicationDbContext context) : IRequestHandler<GetReviewsByProductQuery, PagedResponse<ReviewResponse>>
{
    public async Task<PagedResponse<ReviewResponse>> Handle(GetReviewsByProductQuery request, CancellationToken cancellationToken)
    {
        var projected = context.Reviews
            .AsNoTracking()
            .Where(r => r.ProductId == request.ProductId && r.IsApproved)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReviewResponse(
                r.Id,
                r.Rating,
                r.Title,
                r.Comment,
                r.ProductId,
                r.UserId,
                r.User.FirstName + " " + r.User.LastName,
                r.CreatedAt,
                r.UpdatedAt));

        return await projected.ToPagedResponseAsync(request.Page, request.PageSize, cancellationToken);
    }
}
