using MediatR;
using Organi.Server.Application.Features.Vendors.DTOs;

namespace Organi.Server.Application.Features.Vendors.Commands.RegisterVendor;

public sealed record RegisterVendorCommand(
    string StoreName,
    string? Description,
    string? LogoUrl,
    string? BannerUrl,
    string? PhoneNumber,
    string? Address,
    string? City) : IRequest<VendorResponse>;
