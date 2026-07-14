using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Vendors.Commands.RegisterVendor;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Enums;
using Organi.Server.Domain.Exceptions;
using Xunit;

namespace Organi.Server.UnitTests.Features.Vendors;

public sealed class RegisterVendorHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly ILogger<RegisterVendorHandler> _logger = Substitute.For<ILogger<RegisterVendorHandler>>();
    private readonly RegisterVendorHandler _handler;

    public RegisterVendorHandlerTests()
    {
        _handler = new RegisterVendorHandler(_context, _currentUser, _logger);
    }

    private static RegisterVendorCommand ValidCommand() => new(
        "Green Valley Farm", "Fresh organic produce.", null, null, null, null, null);

    private void SetupVendors(params Vendor[] vendors)
    {
        var mockSet = vendors.ToList().BuildMockDbSet();
        _context.Vendors.Returns(mockSet);
    }

    private void SetupUsers(params User[] users)
    {
        var mockSet = users.ToList().BuildMockDbSet();
        _context.Users.Returns(mockSet);
    }

    private void SetupRoles(params Role[] roles)
    {
        var mockSet = roles.ToList().BuildMockDbSet();
        _context.Roles.Returns(mockSet);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsVendorResponse()
    {
        var userId = Guid.NewGuid();
        _currentUser.UserId.Returns(userId);

        SetupVendors();
        SetupUsers(new User { Id = userId, Email = "vendor@example.com", FirstName = "A", LastName = "B", PasswordHash = "x", Roles = [] });
        SetupRoles(new Role { Name = "Vendor" });

        var result = await _handler.Handle(ValidCommand(), CancellationToken.None);

        result.StoreName.Should().Be("Green Valley Farm");
        result.Slug.Should().Be("green-valley-farm");
        result.Status.Should().Be(nameof(VendorStatus.Pending));
    }

    [Fact]
    public async Task Handle_UserAlreadyHasVendorProfile_ThrowsBusinessRuleException()
    {
        var userId = Guid.NewGuid();
        _currentUser.UserId.Returns(userId);

        SetupVendors(new Vendor { UserId = userId, StoreName = "Existing", Slug = "existing" });

        var act = () => _handler.Handle(ValidCommand(), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*already have a vendor profile*");
    }

    [Fact]
    public async Task Handle_DuplicateStoreName_ThrowsBusinessRuleException()
    {
        var userId = Guid.NewGuid();
        _currentUser.UserId.Returns(userId);

        SetupVendors(new Vendor { UserId = Guid.NewGuid(), StoreName = "Green Valley Farm", Slug = "green-valley-farm" });

        var act = () => _handler.Handle(ValidCommand(), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*already exists*");
    }
}
