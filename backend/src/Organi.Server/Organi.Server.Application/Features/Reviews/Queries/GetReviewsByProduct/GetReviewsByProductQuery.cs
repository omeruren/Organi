using MediatR;
using Organi.Server.Application.Common.Models;
using Organi.Server.Application.Features.Reviews.DTOs;

namespace Organi.Server.Application.Features.Reviews.Queries.GetReviewsByProduct;

public sealed record GetReviewsByProductQuery(Guid ProductId, int Page = 1, int PageSize = 10)
    : IRequest<PagedResponse<ReviewResponse>>;
