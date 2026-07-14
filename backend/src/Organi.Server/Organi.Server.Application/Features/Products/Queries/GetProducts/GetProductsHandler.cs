using MediatR;
using Microsoft.EntityFrameworkCore;
using Organi.Server.Application.Common.Extensions;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Common.Models;
using Organi.Server.Application.Features.Products.DTOs;
using Organi.Server.Domain.Enums;

namespace Organi.Server.Application.Features.Products.Queries.GetProducts;

public sealed class GetProductsHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser) : IRequestHandler<GetProductsQuery, PagedResponse<ProductSummaryResponse>>
{
    public async Task<PagedResponse<ProductSummaryResponse>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var query = context.Products.AsNoTracking();

        var isPrivilegedCaller = currentUser.IsInRole("Admin") || currentUser.IsInRole("Vendor");

        if (isPrivilegedCaller && request.Status is not null)
        {
            var status = Enum.Parse<ProductStatus>(request.Status, ignoreCase: true);
            query = query.Where(p => p.Status == status);
        }
        else if (!isPrivilegedCaller)
        {
            query = query.Where(p => p.Status == ProductStatus.Active);
        }

        if (!currentUser.IsInRole("Admin"))
            query = query.Where(p => p.Vendor.Status == VendorStatus.Approved);

        if (request.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == request.CategoryId);

        if (request.VendorId.HasValue)
            query = query.Where(p => p.VendorId == request.VendorId);

        if (request.MinPrice.HasValue)
            query = query.Where(p => p.Price >= request.MinPrice);

        if (request.MaxPrice.HasValue)
            query = query.Where(p => p.Price <= request.MaxPrice);

        if (request.IsOrganic.HasValue)
            query = query.Where(p => p.IsOrganic == request.IsOrganic);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(p =>
                p.Name.Contains(request.Search) ||
                (p.Description != null && p.Description.Contains(request.Search)));
        }

        var sortBy = request.SortBy?.ToLowerInvariant();
        var descending = string.Equals(request.SortOrder, "desc", StringComparison.OrdinalIgnoreCase);

        query = sortBy switch
        {
            "price" => descending ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
            "rating" => descending ? query.OrderByDescending(p => p.AverageRating) : query.OrderBy(p => p.AverageRating),
            "name" => descending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
            _ => descending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt)
        };

        var projected = query.Select(p => new ProductSummaryResponse(
            p.Id,
            p.Name,
            p.Slug,
            p.Price,
            p.SalePrice,
            p.Unit,
            p.IsOrganic,
            p.Status.ToString(),
            p.CategoryId,
            p.Category.Name,
            p.VendorId,
            p.Vendor.StoreName,
            p.ProductImages.Where(i => i.IsPrimary).Select(i => i.ImageUrl).FirstOrDefault(),
            p.AverageRating));

        return await projected.ToPagedResponseAsync(request.Page, request.PageSize, cancellationToken);
    }
}
