using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Reviews.Commands.UpdateReview;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Exceptions;
using Xunit;

namespace Organi.Server.UnitTests.Features.Reviews;

public sealed class UpdateReviewHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly ILogger<UpdateReviewHandler> _logger = Substitute.For<ILogger<UpdateReviewHandler>>();
    private readonly UpdateReviewHandler _handler;

    public UpdateReviewHandlerTests()
    {
        _handler = new UpdateReviewHandler(_context, _currentUser, _logger);
    }

    private void SetupReviews(params Review[] reviews)
    {
        var mockSet = reviews.ToList().BuildMockDbSet();
        _context.Reviews.Returns(mockSet);
    }

    private static Review BuildReview(Guid userId, int rating = 3)
    {
        var product = new Product { Name = "Honey", Slug = "honey", SKU = "HNY-001" };
        var user = new User { Email = "owner@organi.test", FirstName = "Ada", LastName = "Lovelace", Id = userId };
        return new Review { UserId = userId, User = user, Product = product, ProductId = product.Id, Rating = rating, IsApproved = true };
    }

    [Fact]
    public async Task Handle_Owner_UpdatesReviewAndRecalculatesAggregate()
    {
        var userId = Guid.NewGuid();
        var review = BuildReview(userId, rating: 3);
        SetupReviews(review);
        _currentUser.UserId.Returns(userId);
        _currentUser.IsInRole("Admin").Returns(false);

        var result = await _handler.Handle(new UpdateReviewCommand(5, "Updated", "Better now") { Id = review.Id }, CancellationToken.None);

        result.Rating.Should().Be(5);
        review.Product.AverageRating.Should().Be(5m);
        review.Product.ReviewCount.Should().Be(1);
    }

    [Fact]
    public async Task Handle_NonOwnerNonAdmin_ThrowsForbiddenException()
    {
        var review = BuildReview(Guid.NewGuid());
        SetupReviews(review);
        _currentUser.UserId.Returns(Guid.NewGuid());
        _currentUser.IsInRole("Admin").Returns(false);

        var act = () => _handler.Handle(new UpdateReviewCommand(5, null, null) { Id = review.Id }, CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task Handle_Admin_CanUpdateAnyReview()
    {
        var review = BuildReview(Guid.NewGuid(), rating: 2);
        SetupReviews(review);
        _currentUser.UserId.Returns(Guid.NewGuid());
        _currentUser.IsInRole("Admin").Returns(true);

        var result = await _handler.Handle(new UpdateReviewCommand(4, null, null) { Id = review.Id }, CancellationToken.None);

        result.Rating.Should().Be(4);
    }

    [Fact]
    public async Task Handle_ReviewNotFound_ThrowsNotFoundException()
    {
        SetupReviews();
        _currentUser.UserId.Returns(Guid.NewGuid());

        var act = () => _handler.Handle(new UpdateReviewCommand(5, null, null) { Id = Guid.NewGuid() }, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
