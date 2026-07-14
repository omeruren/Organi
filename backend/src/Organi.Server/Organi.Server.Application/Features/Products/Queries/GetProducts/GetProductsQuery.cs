using MediatR;
using Organi.Server.Application.Common.Models;
using Organi.Server.Application.Features.Products.DTOs;

namespace Organi.Server.Application.Features.Products.Queries.GetProducts;

public sealed record GetProductsQuery(
    Guid? CategoryId,
    Guid? VendorId,
    decimal? MinPrice,
    decimal? MaxPrice,
    bool? IsOrganic,
    string? Status,
    string? Search,
    string? SortBy,
    string? SortOrder,
    int Page = 1,
    int PageSize = 10) : IRequest<PagedResponse<ProductSummaryResponse>>;
