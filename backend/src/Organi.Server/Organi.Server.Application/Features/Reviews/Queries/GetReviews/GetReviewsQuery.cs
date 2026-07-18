using MediatR;
using Organi.Server.Application.Common.Models;
using Organi.Server.Application.Features.Reviews.DTOs;

namespace Organi.Server.Application.Features.Reviews.Queries.GetReviews;

public sealed record GetReviewsQuery(
    Guid? ProductId = null,
    int? Rating = null,
    int Page = 1,
    int PageSize = 10) : IRequest<PagedResponse<AdminReviewResponse>>;
