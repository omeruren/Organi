using MediatR;

namespace Organi.Server.Application.Features.Coupons.Commands.DeleteCoupon;

public sealed record DeleteCouponCommand(Guid Id) : IRequest;
