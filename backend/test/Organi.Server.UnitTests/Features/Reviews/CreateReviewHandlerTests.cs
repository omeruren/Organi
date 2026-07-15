using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Reviews.Commands.CreateReview;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Enums;
using Organi.Server.Domain.Exceptions;
using Xunit;

namespace Organi.Server.UnitTests.Features.Reviews;

public sealed class CreateReviewHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly ILogger<CreateReviewHandler> _logger = Substitute.For<ILogger<CreateReviewHandler>>();
    private readonly CreateReviewHandler _handler;

    public CreateReviewHandlerTests()
    {
        _handler = new CreateReviewHandler(_context, _currentUser, _logger);
    }

    private void SetupProducts(params Product[] products)
    {
        var mockSet = products.ToList().BuildMockDbSet();
        _context.Products.Returns(mockSet);
    }

    private void SetupUsers(params User[] users)
    {
        var mockSet = users.ToList().BuildMockDbSet();
        _context.Users.Returns(mockSet);
    }

    private void SetupOrderItems(params OrderItem[] items)
    {
        var mockSet = items.ToList().BuildMockDbSet();
        _context.OrderItems.Returns(mockSet);
    }

    private void SetupReviews(params Review[] reviews)
    {
        var mockSet = reviews.ToList().BuildMockDbSet();
        _context.Reviews.Returns(mockSet);
    }

    private static (Product product, User user, OrderItem qualifyingItem) BuildQualifyingSetup(OrderStatus status = OrderStatus.Delivered)
    {
        var product = new Product { Name = "Honey", Slug = "honey", SKU = "HNY-001" };
        var user = new User { Email = "buyer@organi.test", FirstName = "Ada", LastName = "Lovelace" };
        var order = new Order { OrderNumber = "ORD-1", UserId = user.Id, Status = status };
        var orderItem = new OrderItem { ProductId = product.Id, Order = order, ProductName = product.Name, ProductSKU = product.SKU };
        return (product, user, orderItem);
    }

    [Fact]
    public async Task Handle_QualifyingPurchase_CreatesReviewAndRecalculatesAggregate()
    {
        var (product, user, orderItem) = BuildQualifyingSetup(OrderStatus.Delivered);
        SetupProducts(product);
        SetupUsers(user);
        SetupOrderItems(orderItem);
        SetupReviews();
        _currentUser.UserId.Returns(user.Id);

        var result = await _handler.Handle(new CreateReviewCommand(5, "Great", "Loved it") { ProductId = product.Id }, CancellationToken.None);

        result.Rating.Should().Be(5);
        product.AverageRating.Should().Be(5m);
        product.ReviewCount.Should().Be(1);
    }

    [Theory]
    [InlineData(OrderStatus.Pending)]
    [InlineData(OrderStatus.Cancelled)]
    [InlineData(OrderStatus.Refunded)]
    public async Task Handle_NonQualifyingOrderStatus_ThrowsBusinessRuleException(OrderStatus status)
    {
        var (product, user, orderItem) = BuildQualifyingSetup(status);
        SetupProducts(product);
        SetupUsers(user);
        SetupOrderItems(orderItem);
        SetupReviews();
        _currentUser.UserId.Returns(user.Id);

        var act = () => _handler.Handle(new CreateReviewCommand(5, null, null) { ProductId = product.Id }, CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*purchased*");
    }

    [Fact]
    public async Task Handle_NoPurchaseAtAll_ThrowsBusinessRuleException()
    {
        var product = new Product { Name = "Honey", Slug = "honey", SKU = "HNY-001" };
        var user = new User { Email = "buyer@organi.test", FirstName = "Ada", LastName = "Lovelace" };
        SetupProducts(product);
        SetupUsers(user);
        SetupOrderItems();
        SetupReviews();
        _currentUser.UserId.Returns(user.Id);

        var act = () => _handler.Handle(new CreateReviewCommand(5, null, null) { ProductId = product.Id }, CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*purchased*");
    }

    [Fact]
    public async Task Handle_AlreadyReviewed_ThrowsBusinessRuleException()
    {
        var (product, user, orderItem) = BuildQualifyingSetup();
        var existingReview = new Review { ProductId = product.Id, UserId = user.Id, Rating = 4, IsApproved = true };
        SetupProducts(product);
        SetupUsers(user);
        SetupOrderItems(orderItem);
        SetupReviews(existingReview);
        _currentUser.UserId.Returns(user.Id);

        var act = () => _handler.Handle(new CreateReviewCommand(5, null, null) { ProductId = product.Id }, CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*already reviewed*");
    }

    [Fact]
    public async Task Handle_ProductNotFound_ThrowsNotFoundException()
    {
        SetupProducts();
        SetupUsers(new User { Email = "buyer@organi.test", FirstName = "Ada", LastName = "Lovelace" });
        SetupOrderItems();
        SetupReviews();
        _currentUser.UserId.Returns(Guid.NewGuid());

        var act = () => _handler.Handle(new CreateReviewCommand(5, null, null) { ProductId = Guid.NewGuid() }, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
