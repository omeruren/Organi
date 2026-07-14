using MediatR;
using Organi.Server.Application.Features.Vendors.DTOs;

namespace Organi.Server.Application.Features.Vendors.Commands.UpdateVendor;

public sealed record UpdateVendorCommand(
    string StoreName,
    string? Description,
    string? LogoUrl,
    string? BannerUrl,
    string? PhoneNumber,
    string? Address,
    string? City) : IRequest<VendorResponse>
{
    public Guid Id { get; init; }
}
