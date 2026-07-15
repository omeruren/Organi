using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Newsletter.Commands.UnsubscribeNewsletter;
using Organi.Server.Domain.Entities;
using Xunit;

namespace Organi.Server.UnitTests.Features.Newsletter;

public sealed class UnsubscribeNewsletterHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ILogger<UnsubscribeNewsletterHandler> _logger = Substitute.For<ILogger<UnsubscribeNewsletterHandler>>();
    private readonly UnsubscribeNewsletterHandler _handler;

    public UnsubscribeNewsletterHandlerTests()
    {
        _handler = new UnsubscribeNewsletterHandler(_context, _logger);
    }

    private void SetupSubscribers(params NewsletterSubscriber[] subscribers)
    {
        var mockSet = subscribers.ToList().BuildMockDbSet();
        _context.NewsletterSubscribers.Returns(mockSet);
    }

    [Fact]
    public async Task Handle_ActiveSubscriber_DeactivatesAndSetsTimestamp()
    {
        var subscriber = new NewsletterSubscriber { Email = "active@organi.test", IsActive = true };
        SetupSubscribers(subscriber);

        await _handler.Handle(new UnsubscribeNewsletterCommand("active@organi.test"), CancellationToken.None);

        subscriber.IsActive.Should().BeFalse();
        subscriber.UnsubscribedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_UnknownEmail_DoesNotThrow()
    {
        SetupSubscribers();

        var act = () => _handler.Handle(new UnsubscribeNewsletterCommand("ghost@organi.test"), CancellationToken.None);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Handle_AlreadyInactiveEmail_DoesNotThrowOrChangeTimestamp()
    {
        var unsubscribedAt = DateTime.UtcNow.AddDays(-2);
        var subscriber = new NewsletterSubscriber { Email = "gone@organi.test", IsActive = false, UnsubscribedAt = unsubscribedAt };
        SetupSubscribers(subscriber);

        await _handler.Handle(new UnsubscribeNewsletterCommand("gone@organi.test"), CancellationToken.None);

        subscriber.UnsubscribedAt.Should().Be(unsubscribedAt);
    }
}
