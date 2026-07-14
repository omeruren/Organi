using MediatR;
using Organi.Server.Application.Common.Models;
using Organi.Server.Application.Features.Products.DTOs;

namespace Organi.Server.Application.Features.Vendors.Queries.GetVendorDashboardProducts;

public sealed record GetVendorDashboardProductsQuery(int Page = 1, int PageSize = 10) : IRequest<PagedResponse<ProductSummaryResponse>>;
