using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Vendors.Commands.SuspendVendor;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Enums;
using Organi.Server.Domain.Exceptions;
using Xunit;

namespace Organi.Server.UnitTests.Features.Vendors;

public sealed class SuspendVendorHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly IAuditService _auditService = Substitute.For<IAuditService>();
    private readonly ILogger<SuspendVendorHandler> _logger = Substitute.For<ILogger<SuspendVendorHandler>>();
    private readonly SuspendVendorHandler _handler;

    public SuspendVendorHandlerTests()
    {
        _handler = new SuspendVendorHandler(_context, _auditService, _logger);
    }

    private void SetupVendors(params Vendor[] vendors)
    {
        var mockSet = vendors.ToList().BuildMockDbSet();
        _context.Vendors.Returns(mockSet);
    }

    [Fact]
    public async Task Handle_VendorNotFound_ThrowsNotFoundException()
    {
        SetupVendors();

        var act = () => _handler.Handle(new SuspendVendorCommand(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_AlreadySuspended_ThrowsBusinessRuleException()
    {
        var vendorId = Guid.NewGuid();
        var vendor = new Vendor { Id = vendorId, StoreName = "Farm", Slug = "farm", Status = VendorStatus.Suspended };

        SetupVendors(vendor);

        var act = () => _handler.Handle(new SuspendVendorCommand(vendorId), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*already suspended*");
    }

    [Fact]
    public async Task Handle_ApprovedVendor_SuspendsAndReturnsResponse()
    {
        var vendorId = Guid.NewGuid();
        var vendor = new Vendor { Id = vendorId, StoreName = "Farm", Slug = "farm", Status = VendorStatus.Approved };

        SetupVendors(vendor);

        var result = await _handler.Handle(new SuspendVendorCommand(vendorId), CancellationToken.None);

        result.Status.Should().Be(nameof(VendorStatus.Suspended));
    }
}
