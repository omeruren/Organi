using MediatR;
using Organi.Server.Application.Common.Models;
using Organi.Server.Application.Features.Coupons.DTOs;

namespace Organi.Server.Application.Features.Coupons.Queries.GetCoupons;

public sealed record GetCouponsQuery(int Page = 1, int PageSize = 10) : IRequest<PagedResponse<CouponResponse>>;
