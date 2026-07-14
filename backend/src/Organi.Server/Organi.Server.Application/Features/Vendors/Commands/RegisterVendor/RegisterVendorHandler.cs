using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Common.Utilities;
using Organi.Server.Application.Features.Vendors.DTOs;
using Organi.Server.Application.Features.Vendors.Mappings;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Enums;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Vendors.Commands.RegisterVendor;

public sealed class RegisterVendorHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser,
    ILogger<RegisterVendorHandler> logger) : IRequestHandler<RegisterVendorCommand, VendorResponse>
{
    private const string VendorRoleName = "Vendor";

    public async Task<VendorResponse> Handle(RegisterVendorCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!.Value;

        var alreadyVendor = await context.Vendors.AnyAsync(v => v.UserId == userId, cancellationToken);
        if (alreadyVendor)
            throw new BusinessRuleException("You already have a vendor profile.");

        var nameExists = await context.Vendors.AnyAsync(v => v.StoreName == request.StoreName, cancellationToken);
        if (nameExists)
            throw new BusinessRuleException($"A vendor named '{request.StoreName}' already exists.");

        var slug = await SlugGenerator.GenerateUniqueAsync(
            request.StoreName,
            candidate => context.Vendors.AnyAsync(v => v.Slug == candidate, cancellationToken));

        var vendor = new Vendor
        {
            StoreName = request.StoreName,
            Slug = slug,
            Description = request.Description,
            LogoUrl = request.LogoUrl,
            BannerUrl = request.BannerUrl,
            PhoneNumber = request.PhoneNumber,
            Address = request.Address,
            City = request.City,
            Status = VendorStatus.Pending,
            UserId = userId
        };

        context.Vendors.Add(vendor);

        var user = await context.Users.Include(u => u.Roles).FirstAsync(u => u.Id == userId, cancellationToken);
        if (!user.Roles.Any(r => r.Name == VendorRoleName))
        {
            var vendorRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == VendorRoleName, cancellationToken)
                ?? throw new BusinessRuleException($"Default role '{VendorRoleName}' is not configured.");
            user.Roles.Add(vendorRole);
        }

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("User {UserId} registered as vendor {VendorId}", userId, vendor.Id);

        return vendor.ToResponse();
    }
}
