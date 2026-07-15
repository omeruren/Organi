using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Vendors.DTOs;
using Organi.Server.Application.Features.Vendors.Mappings;
using Organi.Server.Domain.Enums;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Vendors.Commands.SuspendVendor;

public sealed class SuspendVendorHandler(
    IApplicationDbContext context,
    IAuditService auditService,
    ILogger<SuspendVendorHandler> logger) : IRequestHandler<SuspendVendorCommand, VendorResponse>
{
    public async Task<VendorResponse> Handle(SuspendVendorCommand request, CancellationToken cancellationToken)
    {
        var vendor = await context.Vendors.FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Vendor", request.Id);

        if (vendor.Status == VendorStatus.Suspended)
            throw new BusinessRuleException("Vendor is already suspended.");

        vendor.Status = VendorStatus.Suspended;

        auditService.Log("Vendor", vendor.Id.ToString(), AuditAction.VendorSuspended);

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Vendor {VendorId} suspended", vendor.Id);

        return vendor.ToResponse();
    }
}
