namespace Organi.Server.Application.Features.Vendors.DTOs;

public sealed record VendorResponse(
    Guid Id,
    string StoreName,
    string Slug,
    string? Description,
    string? LogoUrl,
    string? BannerUrl,
    string? PhoneNumber,
    string? Address,
    string? City,
    decimal Rating,
    int TotalSales,
    int FollowerCount,
    string Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
