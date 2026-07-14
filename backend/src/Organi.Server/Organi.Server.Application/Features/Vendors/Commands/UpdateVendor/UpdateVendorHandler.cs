using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Common.Utilities;
using Organi.Server.Application.Features.Vendors.DTOs;
using Organi.Server.Application.Features.Vendors.Mappings;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Vendors.Commands.UpdateVendor;

public sealed class UpdateVendorHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser,
    ILogger<UpdateVendorHandler> logger) : IRequestHandler<UpdateVendorCommand, VendorResponse>
{
    public async Task<VendorResponse> Handle(UpdateVendorCommand request, CancellationToken cancellationToken)
    {
        var vendor = await context.Vendors.FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Vendor", request.Id);

        if (!currentUser.IsInRole("Admin") && vendor.UserId != currentUser.UserId)
            throw new ForbiddenException("You do not have permission to modify this vendor profile.");

        var nameExists = await context.Vendors
            .AnyAsync(v => v.StoreName == request.StoreName && v.Id != vendor.Id, cancellationToken);
        if (nameExists)
            throw new BusinessRuleException($"A vendor named '{request.StoreName}' already exists.");

        if (!string.Equals(request.StoreName, vendor.StoreName, StringComparison.Ordinal))
        {
            vendor.Slug = await SlugGenerator.GenerateUniqueAsync(
                request.StoreName,
                candidate => context.Vendors.AnyAsync(v => v.Slug == candidate && v.Id != vendor.Id, cancellationToken));
        }

        vendor.StoreName = request.StoreName;
        vendor.Description = request.Description;
        vendor.LogoUrl = request.LogoUrl;
        vendor.BannerUrl = request.BannerUrl;
        vendor.PhoneNumber = request.PhoneNumber;
        vendor.Address = request.Address;
        vendor.City = request.City;

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Vendor {VendorId} updated by user {UserId}", vendor.Id, currentUser.UserId);

        return vendor.ToResponse();
    }
}
