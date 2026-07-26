using MediatR;
using Organi.Server.Application.Features.Vendors.DTOs;

namespace Organi.Server.Application.Features.Vendors.Queries.GetVendorBySlug;

public sealed record GetVendorBySlugQuery(string Slug) : IRequest<VendorResponse>;
