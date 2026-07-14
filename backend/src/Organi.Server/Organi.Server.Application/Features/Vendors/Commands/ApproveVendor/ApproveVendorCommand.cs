using MediatR;
using Organi.Server.Application.Features.Vendors.DTOs;

namespace Organi.Server.Application.Features.Vendors.Commands.ApproveVendor;

public sealed record ApproveVendorCommand(Guid Id) : IRequest<VendorResponse>;
