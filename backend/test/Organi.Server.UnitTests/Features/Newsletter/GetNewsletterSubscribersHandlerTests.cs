using FluentAssertions;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Newsletter.Queries.GetNewsletterSubscribers;
using Organi.Server.Domain.Entities;
using Xunit;

namespace Organi.Server.UnitTests.Features.Newsletter;

public sealed class GetNewsletterSubscribersHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly GetNewsletterSubscribersHandler _handler;

    public GetNewsletterSubscribersHandlerTests()
    {
        _handler = new GetNewsletterSubscribersHandler(_context);
    }

    private void SetupSubscribers(params NewsletterSubscriber[] subscribers)
    {
        var mockSet = subscribers.ToList().BuildMockDbSet();
        _context.NewsletterSubscribers.Returns(mockSet);
    }

    [Fact]
    public async Task Handle_NoSubscribers_ReturnsEmptyPage()
    {
        SetupSubscribers();

        var result = await _handler.Handle(new GetNewsletterSubscribersQuery(), CancellationToken.None);

        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_MultipleSubscribers_OrdersByMostRecentFirst()
    {
        var older = new NewsletterSubscriber { Email = "older@organi.test", SubscribedAt = DateTime.UtcNow.AddDays(-10) };
        var newer = new NewsletterSubscriber { Email = "newer@organi.test", SubscribedAt = DateTime.UtcNow };
        SetupSubscribers(older, newer);

        var result = await _handler.Handle(new GetNewsletterSubscribersQuery(), CancellationToken.None);

        result.Items.Should().HaveCount(2);
        result.Items[0].Email.Should().Be("newer@organi.test");
        result.Items[1].Email.Should().Be("older@organi.test");
    }

    [Fact]
    public async Task Handle_IsActiveFilter_ReturnsOnlyMatching()
    {
        var active = new NewsletterSubscriber { Email = "active@organi.test", IsActive = true, SubscribedAt = DateTime.UtcNow };
        var inactive = new NewsletterSubscriber { Email = "inactive@organi.test", IsActive = false, SubscribedAt = DateTime.UtcNow, UnsubscribedAt = DateTime.UtcNow };
        SetupSubscribers(active, inactive);

        var result = await _handler.Handle(new GetNewsletterSubscribersQuery(IsActive: true), CancellationToken.None);

        result.Items.Should().ContainSingle();
        result.Items[0].Email.Should().Be("active@organi.test");
    }

    [Fact]
    public async Task Handle_SearchFiltersByEmail()
    {
        var match = new NewsletterSubscriber { Email = "findme@organi.test", SubscribedAt = DateTime.UtcNow };
        var noMatch = new NewsletterSubscriber { Email = "other@organi.test", SubscribedAt = DateTime.UtcNow };
        SetupSubscribers(match, noMatch);

        var result = await _handler.Handle(new GetNewsletterSubscribersQuery(Search: "findme"), CancellationToken.None);

        result.Items.Should().ContainSingle();
        result.Items[0].Email.Should().Be("findme@organi.test");
    }
}
