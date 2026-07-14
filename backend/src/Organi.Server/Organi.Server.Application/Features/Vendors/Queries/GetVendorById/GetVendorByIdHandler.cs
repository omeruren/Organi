using MediatR;
using Microsoft.EntityFrameworkCore;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Vendors.DTOs;
using Organi.Server.Application.Features.Vendors.Mappings;
using Organi.Server.Domain.Enums;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Vendors.Queries.GetVendorById;

public sealed class GetVendorByIdHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser) : IRequestHandler<GetVendorByIdQuery, VendorResponse>
{
    public async Task<VendorResponse> Handle(GetVendorByIdQuery request, CancellationToken cancellationToken)
    {
        var vendor = await context.Vendors
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Vendor", request.Id);

        var isOwnerOrAdmin = currentUser.IsInRole("Admin") || vendor.UserId == currentUser.UserId;
        if (!isOwnerOrAdmin && vendor.Status != VendorStatus.Approved)
            throw new NotFoundException("Vendor", request.Id);

        return vendor.ToResponse();
    }
}
