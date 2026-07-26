using MediatR;
using Microsoft.EntityFrameworkCore;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Vendors.DTOs;
using Organi.Server.Application.Features.Vendors.Mappings;
using Organi.Server.Domain.Enums;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Vendors.Queries.GetVendorBySlug;

public sealed class GetVendorBySlugHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser) : IRequestHandler<GetVendorBySlugQuery, VendorResponse>
{
    public async Task<VendorResponse> Handle(GetVendorBySlugQuery request, CancellationToken cancellationToken)
    {
        var vendor = await context.Vendors
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Slug == request.Slug, cancellationToken)
            ?? throw new NotFoundException("Vendor", request.Slug);

        var isOwnerOrAdmin = currentUser.IsInRole("Admin") || vendor.UserId == currentUser.UserId;
        if (!isOwnerOrAdmin && vendor.Status != VendorStatus.Approved)
            throw new NotFoundException("Vendor", request.Slug);

        return vendor.ToResponse();
    }
}
