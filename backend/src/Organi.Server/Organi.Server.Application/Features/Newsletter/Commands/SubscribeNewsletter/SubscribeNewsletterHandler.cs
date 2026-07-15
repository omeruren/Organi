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
    ILogger<SubscribeNewsletterHandler> logger) : IRequestHandler<SubscribeNewsletterCommand, NewsletterSubscriberResponse>
{
    public async Task<NewsletterSubscriberResponse> Handle(SubscribeNewsletterCommand request, CancellationToken cancellationToken)
    {
        var subscriber = await context.NewsletterSubscribers
            .FirstOrDefaultAsync(n => n.Email == request.Email, cancellationToken);

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

            logger.LogInformation("Email {Email} subscribed to the newsletter", subscriber.Email);
        }
        else if (!subscriber.IsActive)
        {
            subscriber.IsActive = true;
            subscriber.UnsubscribedAt = null;

            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Email {Email} resubscribed to the newsletter", subscriber.Email);
        }

        return subscriber.ToResponse();
    }
}
