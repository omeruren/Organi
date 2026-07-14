using MediatR;
using Organi.Server.Application.Features.Coupons.DTOs;

namespace Organi.Server.Application.Features.Coupons.Queries.ValidateCoupon;

public sealed record ValidateCouponQuery(string Code) : IRequest<CouponValidationResponse>;
