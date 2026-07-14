using MediatR;
using Microsoft.EntityFrameworkCore;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Products.DTOs;
using Organi.Server.Domain.Enums;

namespace Organi.Server.Application.Features.Products.Queries.GetFeaturedProducts;

public sealed class GetFeaturedProductsHandler(
    IApplicationDbContext context) : IRequestHandler<GetFeaturedProductsQuery, IReadOnlyList<ProductSummaryResponse>>
{
    public async Task<IReadOnlyList<ProductSummaryResponse>> Handle(GetFeaturedProductsQuery request, CancellationToken cancellationToken)
    {
        return await context.Products
            .AsNoTracking()
            .Where(p => p.IsFeatured && p.Status == ProductStatus.Active && p.Vendor.Status == VendorStatus.Approved)
            .OrderByDescending(p => p.AverageRating)
            .ThenByDescending(p => p.CreatedAt)
            .Take(request.Take)
            .Select(p => new ProductSummaryResponse(
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
                p.AverageRating))
            .ToListAsync(cancellationToken);
    }
}
