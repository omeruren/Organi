using MediatR;
using Organi.Server.Application.Features.Coupons.DTOs;

namespace Organi.Server.Application.Features.Coupons.Queries.GetCouponById;

public sealed record GetCouponByIdQuery(Guid Id) : IRequest<CouponResponse>;
