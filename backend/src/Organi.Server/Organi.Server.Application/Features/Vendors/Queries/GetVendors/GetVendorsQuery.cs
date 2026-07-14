using MediatR;
using Organi.Server.Application.Common.Models;
using Organi.Server.Application.Features.Vendors.DTOs;

namespace Organi.Server.Application.Features.Vendors.Queries.GetVendors;

public sealed record GetVendorsQuery(int Page = 1, int PageSize = 10) : IRequest<PagedResponse<VendorResponse>>;
