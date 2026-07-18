using FluentAssertions;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Reviews.Queries.GetReviews;
using Organi.Server.Domain.Entities;
using Xunit;

namespace Organi.Server.UnitTests.Features.Reviews;

public sealed class GetReviewsHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly GetReviewsHandler _handler;

    public GetReviewsHandlerTests()
    {
        _handler = new GetReviewsHandler(_context);
    }

    private void SetupReviews(params Review[] reviews)
    {
        var mockSet = reviews.ToList().BuildMockDbSet();
        _context.Reviews.Returns(mockSet);
    }

    private static Review BuildReview(string productName, int rating, bool isApproved = true, DateTime? createdAt = null)
    {
        var product = new Product { Name = productName, Slug = productName.ToLowerInvariant(), SKU = $"SKU-{productName}" };
        var user = new User { Email = "reviewer@organi.test", FirstName = "Ada", LastName = "Lovelace" };

        return new Review
        {
            Rating = rating,
            Title = $"Review of {productName}",
            IsApproved = isApproved,
            Product = product,
            ProductId = product.Id,
            User = user,
            UserId = user.Id,
            CreatedAt = createdAt ?? DateTime.UtcNow
        };
    }

    [Fact]
    public async Task Handle_NoFilters_ReturnsAllReviewsIncludingUnapproved()
    {
        var approved = BuildReview("Honey", 5, isApproved: true);
        var unapproved = BuildReview("Jam", 2, isApproved: false);
        SetupReviews(approved, unapproved);

        var result = await _handler.Handle(new GetReviewsQuery(), CancellationToken.None);

        result.Items.Should().HaveCount(2, "moderation view must not hide unapproved reviews");
    }

    [Fact]
    public async Task Handle_ProductIdFilter_ReturnsOnlyThatProductsReviews()
    {
        var honeyReview = BuildReview("Honey", 5);
        var jamReview = BuildReview("Jam", 3);
        SetupReviews(honeyReview, jamReview);

        var result = await _handler.Handle(new GetReviewsQuery(ProductId: honeyReview.ProductId), CancellationToken.None);

        result.Items.Should().ContainSingle();
        result.Items[0].ProductName.Should().Be("Honey");
    }

    [Fact]
    public async Task Handle_RatingFilter_ReturnsOnlyMatchingRating()
    {
        var fiveStar = BuildReview("Honey", 5);
        var oneStar = BuildReview("Jam", 1);
        SetupReviews(fiveStar, oneStar);

        var result = await _handler.Handle(new GetReviewsQuery(Rating: 1), CancellationToken.None);

        result.Items.Should().ContainSingle();
        result.Items[0].Rating.Should().Be(1);
    }

    [Fact]
    public async Task Handle_MultipleReviews_OrdersByMostRecentFirst()
    {
        var older = BuildReview("Honey", 4, createdAt: DateTime.UtcNow.AddDays(-5));
        var newer = BuildReview("Jam", 3, createdAt: DateTime.UtcNow);
        SetupReviews(older, newer);

        var result = await _handler.Handle(new GetReviewsQuery(), CancellationToken.None);

        result.Items[0].ProductName.Should().Be("Jam");
        result.Items[1].ProductName.Should().Be("Honey");
    }

    [Fact]
    public async Task Handle_ProjectsProductNameAndUserFullName()
    {
        SetupReviews(BuildReview("Honey", 5));

        var result = await _handler.Handle(new GetReviewsQuery(), CancellationToken.None);

        result.Items[0].ProductName.Should().Be("Honey");
        result.Items[0].UserFullName.Should().Be("Ada Lovelace");
    }
}
