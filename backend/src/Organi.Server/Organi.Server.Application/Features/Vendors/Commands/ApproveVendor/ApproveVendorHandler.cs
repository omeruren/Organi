using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Vendors.DTOs;
using Organi.Server.Application.Features.Vendors.Mappings;
using Organi.Server.Domain.Enums;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Vendors.Commands.ApproveVendor;

public sealed class ApproveVendorHandler(
    IApplicationDbContext context,
    ILogger<ApproveVendorHandler> logger) : IRequestHandler<ApproveVendorCommand, VendorResponse>
{
    public async Task<VendorResponse> Handle(ApproveVendorCommand request, CancellationToken cancellationToken)
    {
        var vendor = await context.Vendors.FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Vendor", request.Id);

        if (vendor.Status == VendorStatus.Approved)
            throw new BusinessRuleException("Vendor is already approved.");

        vendor.Status = VendorStatus.Approved;
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Vendor {VendorId} approved", vendor.Id);

        return vendor.ToResponse();
    }
}
