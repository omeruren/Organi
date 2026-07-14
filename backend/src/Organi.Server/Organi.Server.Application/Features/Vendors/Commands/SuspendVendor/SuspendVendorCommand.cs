using MediatR;
using Organi.Server.Application.Features.Vendors.DTOs;

namespace Organi.Server.Application.Features.Vendors.Commands.SuspendVendor;

public sealed record SuspendVendorCommand(Guid Id) : IRequest<VendorResponse>;
