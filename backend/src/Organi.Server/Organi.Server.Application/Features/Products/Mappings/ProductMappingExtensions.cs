using Organi.Server.Application.Features.Products.DTOs;
using Organi.Server.Domain.Entities;

namespace Organi.Server.Application.Features.Products.Mappings;

public static class ProductMappingExtensions
{
    public static ProductResponse ToResponse(this Product product) => new(
        product.Id,
        product.Name,
        product.Slug,
        product.Description,
        product.ShortDescription,
        product.Price,
        product.SalePrice,
        product.SKU,
        product.StockQuantity,
        product.Unit,
        product.Weight,
        product.IsOrganic,
        product.IsFeatured,
        product.Status.ToString(),
        product.AverageRating,
        product.ReviewCount,
        product.CategoryId,
        product.Category.Name,
        product.VendorId,
        product.Vendor.StoreName,
        product.ProductImages
            .OrderBy(i => i.DisplayOrder)
            .Select(i => new ProductImageResponse(i.Id, i.ImageUrl, i.AltText, i.DisplayOrder, i.IsPrimary))
            .ToList(),
        product.CreatedAt,
        product.UpdatedAt);
}
