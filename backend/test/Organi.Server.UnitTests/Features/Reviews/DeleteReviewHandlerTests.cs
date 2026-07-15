using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Reviews.Commands.DeleteReview;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Exceptions;
using Xunit;

namespace Organi.Server.UnitTests.Features.Reviews;

public sealed class DeleteReviewHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly ILogger<DeleteReviewHandler> _logger = Substitute.For<ILogger<DeleteReviewHandler>>();
    private readonly DeleteReviewHandler _handler;

    public DeleteReviewHandlerTests()
    {
        _handler = new DeleteReviewHandler(_context, _currentUser, _logger);
    }

    private void SetupReviews(params Review[] reviews)
    {
        var mockSet = reviews.ToList().BuildMockDbSet();
        _context.Reviews.Returns(mockSet);
    }

    private static Review BuildReview(Guid userId, Product product, int rating = 4) =>
        new() { UserId = userId, Product = product, ProductId = product.Id, Rating = rating, IsApproved = true };

    [Fact]
    public async Task Handle_Owner_RemovesReviewAndRecalculatesAggregate()
    {
        var userId = Guid.NewGuid();
        var product = new Product { Name = "Honey", Slug = "honey", SKU = "HNY-001", AverageRating = 4m, ReviewCount = 1 };
        var review = BuildReview(userId, product);
        SetupReviews(review);
        _currentUser.UserId.Returns(userId);
        _currentUser.IsInRole("Admin").Returns(false);

        await _handler.Handle(new DeleteReviewCommand(review.Id), CancellationToken.None);

        _context.Reviews.Received(1).Remove(review);
        product.AverageRating.Should().Be(0m);
        product.ReviewCount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_NonOwnerNonAdmin_ThrowsForbiddenException()
    {
        var product = new Product { Name = "Honey", Slug = "honey", SKU = "HNY-001" };
        var review = BuildReview(Guid.NewGuid(), product);
        SetupReviews(review);
        _currentUser.UserId.Returns(Guid.NewGuid());
        _currentUser.IsInRole("Admin").Returns(false);

        var act = () => _handler.Handle(new DeleteReviewCommand(review.Id), CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task Handle_ReviewNotFound_ThrowsNotFoundException()
    {
        SetupReviews();
        _currentUser.UserId.Returns(Guid.NewGuid());

        var act = () => _handler.Handle(new DeleteReviewCommand(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
