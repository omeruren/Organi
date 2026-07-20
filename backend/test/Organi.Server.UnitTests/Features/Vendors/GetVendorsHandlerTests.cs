using FluentAssertions;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Vendors.Queries.GetVendors;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Enums;
using Xunit;

namespace Organi.Server.UnitTests.Features.Vendors;

public sealed class GetVendorsHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly GetVendorsHandler _handler;

    public GetVendorsHandlerTests()
    {
        _handler = new GetVendorsHandler(_context, _currentUser);
    }

    private void SetupVendors(params Vendor[] vendors)
    {
        var mockSet = vendors.ToList().BuildMockDbSet();
        _context.Vendors.Returns(mockSet);
    }

    private static Vendor BuildVendor(string storeName, VendorStatus status) =>
        new() { StoreName = storeName, Slug = storeName.ToLowerInvariant().Replace(' ', '-'), Status = status };

    [Fact]
    public async Task Handle_NonAdmin_ReturnsOnlyApprovedVendors()
    {
        _currentUser.IsInRole("Admin").Returns(false);
        SetupVendors(
            BuildVendor("Approved Store", VendorStatus.Approved),
            BuildVendor("Pending Store", VendorStatus.Pending));

        var result = await _handler.Handle(new GetVendorsQuery(), CancellationToken.None);

        result.Items.Should().ContainSingle();
        result.Items[0].StoreName.Should().Be("Approved Store");
    }

    [Fact]
    public async Task Handle_NonAdmin_StatusFilterIsIgnored()
    {
        _currentUser.IsInRole("Admin").Returns(false);
        SetupVendors(
            BuildVendor("Approved Store", VendorStatus.Approved),
            BuildVendor("Pending Store", VendorStatus.Pending));

        var result = await _handler.Handle(new GetVendorsQuery(Status: "Pending"), CancellationToken.None);

        result.Items.Should().ContainSingle();
        result.Items[0].StoreName.Should().Be("Approved Store");
    }

    [Fact]
    public async Task Handle_Admin_SeesAllStatuses()
    {
        _currentUser.IsInRole("Admin").Returns(true);
        SetupVendors(
            BuildVendor("Approved Store", VendorStatus.Approved),
            BuildVendor("Pending Store", VendorStatus.Pending),
            BuildVendor("Suspended Store", VendorStatus.Suspended));

        var result = await _handler.Handle(new GetVendorsQuery(), CancellationToken.None);

        result.Items.Should().HaveCount(3);
    }

    [Fact]
    public async Task Handle_Admin_StatusFilter_ReturnsOnlyMatching()
    {
        _currentUser.IsInRole("Admin").Returns(true);
        SetupVendors(
            BuildVendor("Approved Store", VendorStatus.Approved),
            BuildVendor("Pending Store", VendorStatus.Pending));

        var result = await _handler.Handle(new GetVendorsQuery(Status: "pending"), CancellationToken.None);

        result.Items.Should().ContainSingle();
        result.Items[0].StoreName.Should().Be("Pending Store");
    }

    [Fact]
    public async Task Handle_SearchFiltersByStoreName()
    {
        _currentUser.IsInRole("Admin").Returns(true);
        SetupVendors(
            BuildVendor("Green Valley Farm", VendorStatus.Approved),
            BuildVendor("Blue Hill Dairy", VendorStatus.Approved));

        var result = await _handler.Handle(new GetVendorsQuery(Search: "Valley"), CancellationToken.None);

        result.Items.Should().ContainSingle();
        result.Items[0].StoreName.Should().Be("Green Valley Farm");
    }
}
