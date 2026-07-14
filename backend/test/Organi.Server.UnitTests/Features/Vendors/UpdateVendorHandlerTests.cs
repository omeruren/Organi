using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Vendors.Commands.UpdateVendor;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Exceptions;
using Xunit;

namespace Organi.Server.UnitTests.Features.Vendors;

public sealed class UpdateVendorHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly ILogger<UpdateVendorHandler> _logger = Substitute.For<ILogger<UpdateVendorHandler>>();
    private readonly UpdateVendorHandler _handler;

    public UpdateVendorHandlerTests()
    {
        _handler = new UpdateVendorHandler(_context, _currentUser, _logger);
    }

    private static UpdateVendorCommand ValidCommand(Guid id) => new(
        "Green Valley Farm Updated", null, null, null, null, null, null)
    { Id = id };

    private void SetupVendors(params Vendor[] vendors)
    {
        var mockSet = vendors.ToList().BuildMockDbSet();
        _context.Vendors.Returns(mockSet);
    }

    [Fact]
    public async Task Handle_VendorNotFound_ThrowsNotFoundException()
    {
        SetupVendors();

        var act = () => _handler.Handle(ValidCommand(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_NotOwnerNotAdmin_ThrowsForbiddenException()
    {
        var vendorId = Guid.NewGuid();
        var ownerUserId = Guid.NewGuid();
        var vendor = new Vendor { Id = vendorId, StoreName = "Green Valley Farm", Slug = "green-valley-farm", UserId = ownerUserId };

        SetupVendors(vendor);
        _currentUser.UserId.Returns(Guid.NewGuid());
        _currentUser.IsInRole("Admin").Returns(false);

        var act = () => _handler.Handle(ValidCommand(vendorId), CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task Handle_DuplicateStoreNameFromAnotherVendor_ThrowsBusinessRuleException()
    {
        var vendorId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var vendor = new Vendor { Id = vendorId, StoreName = "Old Name", Slug = "old-name", UserId = userId };
        var otherVendor = new Vendor { Id = Guid.NewGuid(), StoreName = "Green Valley Farm Updated", Slug = "green-valley-farm-updated" };

        SetupVendors(vendor, otherVendor);
        _currentUser.UserId.Returns(userId);
        _currentUser.IsInRole("Admin").Returns(false);

        var act = () => _handler.Handle(ValidCommand(vendorId), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*already exists*");
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsUpdatedVendorResponse()
    {
        var vendorId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var vendor = new Vendor { Id = vendorId, StoreName = "Old Name", Slug = "old-name", UserId = userId };

        SetupVendors(vendor);
        _currentUser.UserId.Returns(userId);
        _currentUser.IsInRole("Admin").Returns(false);

        var result = await _handler.Handle(ValidCommand(vendorId), CancellationToken.None);

        result.StoreName.Should().Be("Green Valley Farm Updated");
        result.Slug.Should().Be("green-valley-farm-updated");
    }
}
