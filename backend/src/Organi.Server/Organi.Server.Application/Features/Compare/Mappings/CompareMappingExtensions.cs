using Organi.Server.Application.Features.Compare.DTOs;
using Organi.Server.Domain.Entities;

namespace Organi.Server.Application.Features.Compare.Mappings;

public static class CompareMappingExtensions
{
    public static CompareItemResponse ToResponse(this CompareItem item) => new(
        item.Id,
        item.ProductId,
        item.Product.Name,
        item.Product.Slug,
        item.Product.Price,
        item.Product.SalePrice,
        item.Product.ProductImages.Where(i => i.IsPrimary).Select(i => i.ImageUrl).FirstOrDefault(),
        item.CreatedAt);
}
