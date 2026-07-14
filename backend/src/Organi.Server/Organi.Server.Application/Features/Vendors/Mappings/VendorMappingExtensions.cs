using Organi.Server.Application.Features.Vendors.DTOs;
using Organi.Server.Domain.Entities;

namespace Organi.Server.Application.Features.Vendors.Mappings;

public static class VendorMappingExtensions
{
    public static VendorResponse ToResponse(this Vendor vendor) => new(
        vendor.Id,
        vendor.StoreName,
        vendor.Slug,
        vendor.Description,
        vendor.LogoUrl,
        vendor.BannerUrl,
        vendor.PhoneNumber,
        vendor.Address,
        vendor.City,
        vendor.Rating,
        vendor.TotalSales,
        vendor.FollowerCount,
        vendor.Status.ToString(),
        vendor.CreatedAt,
        vendor.UpdatedAt);
}
