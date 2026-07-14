using MediatR;
using Organi.Server.Application.Features.Vendors.DTOs;

namespace Organi.Server.Application.Features.Vendors.Queries.GetVendorById;

public sealed record GetVendorByIdQuery(Guid Id) : IRequest<VendorResponse>;
