using FluentAssertions;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Reviews.Queries.GetReviewsByProduct;
using Organi.Server.Domain.Entities;
using Xunit;

namespace Organi.Server.UnitTests.Features.Reviews;

public sealed class GetReviewsByProductHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly GetReviewsByProductHandler _handler;

    public GetReviewsByProductHandlerTests()
    {
        _handler = new GetReviewsByProductHandler(_context);
    }

    private void SetupReviews(params Review[] reviews)
    {
        var mockSet = reviews.ToList().BuildMockDbSet();
        _context.Reviews.Returns(mockSet);
    }

    [Fact]
    public async Task Handle_OnlyApprovedReviewsForProduct_AreReturned()
    {
        var productId = Guid.NewGuid();
        var user = new User { Email = "a@organi.test", FirstName = "Ada", LastName = "Lovelace" };
        var approved = new Review { ProductId = productId, User = user, IsApproved = true, Rating = 5, CreatedAt = DateTime.UtcNow };
        var unapproved = new Review { ProductId = productId, User = user, IsApproved = false, Rating = 1, CreatedAt = DateTime.UtcNow };
        var otherProduct = new Review { ProductId = Guid.NewGuid(), User = user, IsApproved = true, Rating = 3, CreatedAt = DateTime.UtcNow };
        SetupReviews(approved, unapproved, otherProduct);

        var result = await _handler.Handle(new GetReviewsByProductQuery(productId), CancellationToken.None);

        result.Items.Should().ContainSingle();
        result.Items[0].Rating.Should().Be(5);
    }

    [Fact]
    public async Task Handle_MultipleReviews_OrdersByMostRecentFirst()
    {
        var productId = Guid.NewGuid();
        var user = new User { Email = "a@organi.test", FirstName = "Ada", LastName = "Lovelace" };
        var older = new Review { ProductId = productId, User = user, IsApproved = true, Rating = 3, CreatedAt = DateTime.UtcNow.AddDays(-5) };
        var newer = new Review { ProductId = productId, User = user, IsApproved = true, Rating = 5, CreatedAt = DateTime.UtcNow };
        SetupReviews(older, newer);

        var result = await _handler.Handle(new GetReviewsByProductQuery(productId), CancellationToken.None);

        result.Items.Should().HaveCount(2);
        result.Items[0].Rating.Should().Be(5);
        result.Items[1].Rating.Should().Be(3);
    }
}
