using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Newsletter.Commands.SubscribeNewsletter;
using Organi.Server.Domain.Entities;
using Xunit;

namespace Organi.Server.UnitTests.Features.Newsletter;

public sealed class SubscribeNewsletterHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ILogger<SubscribeNewsletterHandler> _logger = Substitute.For<ILogger<SubscribeNewsletterHandler>>();
    private readonly SubscribeNewsletterHandler _handler;

    public SubscribeNewsletterHandlerTests()
    {
        _handler = new SubscribeNewsletterHandler(_context, _logger);
    }

    private void SetupSubscribers(params NewsletterSubscriber[] subscribers)
    {
        var mockSet = subscribers.ToList().BuildMockDbSet();
        _context.NewsletterSubscribers.Returns(mockSet);
    }

    [Fact]
    public async Task Handle_NewEmail_CreatesActiveSubscriber()
    {
        SetupSubscribers();

        var result = await _handler.Handle(new SubscribeNewsletterCommand("new@organi.test"), CancellationToken.None);

        result.Email.Should().Be("new@organi.test");
        result.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_AlreadyActiveEmail_ReturnsExistingWithoutChange()
    {
        var subscribedAt = DateTime.UtcNow.AddDays(-10);
        var existing = new NewsletterSubscriber { Email = "active@organi.test", IsActive = true, SubscribedAt = subscribedAt };
        SetupSubscribers(existing);

        var result = await _handler.Handle(new SubscribeNewsletterCommand("active@organi.test"), CancellationToken.None);

        result.IsActive.Should().BeTrue();
        result.SubscribedAt.Should().Be(subscribedAt);
    }

    [Fact]
    public async Task Handle_PreviouslyUnsubscribedEmail_Reactivates()
    {
        var subscribedAt = DateTime.UtcNow.AddDays(-30);
        var existing = new NewsletterSubscriber
        {
            Email = "back@organi.test",
            IsActive = false,
            SubscribedAt = subscribedAt,
            UnsubscribedAt = DateTime.UtcNow.AddDays(-5)
        };
        SetupSubscribers(existing);

        var result = await _handler.Handle(new SubscribeNewsletterCommand("back@organi.test"), CancellationToken.None);

        result.IsActive.Should().BeTrue();
        result.UnsubscribedAt.Should().BeNull();
        result.SubscribedAt.Should().Be(subscribedAt, "original signup date is preserved on resubscribe");
    }
}
