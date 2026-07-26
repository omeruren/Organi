using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Newsletter.DTOs;
using Organi.Server.Application.Features.Newsletter.Mappings;
using Organi.Server.Domain.Entities;

namespace Organi.Server.Application.Features.Newsletter.Commands.SubscribeNewsletter;

public sealed class SubscribeNewsletterHandler(
    IApplicationDbContext context,
    IEmailService emailService,
    ILogger<SubscribeNewsletterHandler> logger) : IRequestHandler<SubscribeNewsletterCommand, NewsletterSubscriberResponse>
{
    public async Task<NewsletterSubscriberResponse> Handle(SubscribeNewsletterCommand request, CancellationToken cancellationToken)
    {
        var subscriber = await context.NewsletterSubscribers
            .FirstOrDefaultAsync(n => n.Email == request.Email, cancellationToken);

        // Only the two mutating branches send a welcome. Re-submitting an already-active address
        // is a silent no-op, so repeat submitters aren't mailed every time they hit Subscribe.
        var subscriptionChanged = false;

        if (subscriber is null)
        {
            subscriber = new NewsletterSubscriber
            {
                Email = request.Email,
                IsConfirmed = true,
                IsActive = true,
                SubscribedAt = DateTime.UtcNow
            };

            context.NewsletterSubscribers.Add(subscriber);
            await context.SaveChangesAsync(cancellationToken);
            subscriptionChanged = true;

            logger.LogInformation("Email {Email} subscribed to the newsletter", subscriber.Email);
        }
        else if (!subscriber.IsActive)
        {
            subscriber.IsActive = true;
            subscriber.UnsubscribedAt = null;

            await context.SaveChangesAsync(cancellationToken);
            subscriptionChanged = true;

            logger.LogInformation("Email {Email} resubscribed to the newsletter", subscriber.Email);
        }

        if (subscriptionChanged)
            await emailService.SendNewsletterWelcomeAsync(subscriber.Email, CancellationToken.None);

        return subscriber.ToResponse();
    }
}
