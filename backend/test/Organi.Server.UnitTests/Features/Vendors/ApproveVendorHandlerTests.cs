using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Vendors.Commands.ApproveVendor;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Enums;
using Organi.Server.Domain.Exceptions;
using Xunit;

namespace Organi.Server.UnitTests.Features.Vendors;

public sealed class ApproveVendorHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ILogger<ApproveVendorHandler> _logger = Substitute.For<ILogger<ApproveVendorHandler>>();
    private readonly ApproveVendorHandler _handler;

    public ApproveVendorHandlerTests()
    {
        _handler = new ApproveVendorHandler(_context, _logger);
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

        var act = () => _handler.Handle(new ApproveVendorCommand(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_AlreadyApproved_ThrowsBusinessRuleException()
    {
        var vendorId = Guid.NewGuid();
        var vendor = new Vendor { Id = vendorId, StoreName = "Farm", Slug = "farm", Status = VendorStatus.Approved };

        SetupVendors(vendor);

        var act = () => _handler.Handle(new ApproveVendorCommand(vendorId), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*already approved*");
    }

    [Fact]
    public async Task Handle_PendingVendor_ApprovesAndReturnsResponse()
    {
        var vendorId = Guid.NewGuid();
        var vendor = new Vendor { Id = vendorId, StoreName = "Farm", Slug = "farm", Status = VendorStatus.Pending };

        SetupVendors(vendor);

        var result = await _handler.Handle(new ApproveVendorCommand(vendorId), CancellationToken.None);

        result.Status.Should().Be(nameof(VendorStatus.Approved));
    }
}
