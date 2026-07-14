using FluentAssertions;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Vendors.Queries.GetVendorById;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Enums;
using Organi.Server.Domain.Exceptions;
using Xunit;

namespace Organi.Server.UnitTests.Features.Vendors;

public sealed class GetVendorByIdHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly GetVendorByIdHandler _handler;

    public GetVendorByIdHandlerTests()
    {
        _handler = new GetVendorByIdHandler(_context, _currentUser);
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

        var act = () => _handler.Handle(new GetVendorByIdQuery(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_PendingVendorAsAnonymous_ThrowsNotFoundException()
    {
        var vendorId = Guid.NewGuid();
        var vendor = new Vendor { Id = vendorId, StoreName = "Farm", Slug = "farm", Status = VendorStatus.Pending, UserId = Guid.NewGuid() };

        SetupVendors(vendor);
        _currentUser.IsInRole("Admin").Returns(false);
        _currentUser.UserId.Returns((Guid?)null);

        var act = () => _handler.Handle(new GetVendorByIdQuery(vendorId), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_PendingVendorAsOwner_ReturnsVendor()
    {
        var vendorId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var vendor = new Vendor { Id = vendorId, StoreName = "Farm", Slug = "farm", Status = VendorStatus.Pending, UserId = userId };

        SetupVendors(vendor);
        _currentUser.IsInRole("Admin").Returns(false);
        _currentUser.UserId.Returns(userId);

        var result = await _handler.Handle(new GetVendorByIdQuery(vendorId), CancellationToken.None);

        result.Id.Should().Be(vendorId);
    }

    [Fact]
    public async Task Handle_ApprovedVendor_ReturnsVendorToAnyone()
    {
        var vendorId = Guid.NewGuid();
        var vendor = new Vendor { Id = vendorId, StoreName = "Farm", Slug = "farm", Status = VendorStatus.Approved, UserId = Guid.NewGuid() };

        SetupVendors(vendor);
        _currentUser.IsInRole("Admin").Returns(false);
        _currentUser.UserId.Returns((Guid?)null);

        var result = await _handler.Handle(new GetVendorByIdQuery(vendorId), CancellationToken.None);

        result.Id.Should().Be(vendorId);
    }
}
