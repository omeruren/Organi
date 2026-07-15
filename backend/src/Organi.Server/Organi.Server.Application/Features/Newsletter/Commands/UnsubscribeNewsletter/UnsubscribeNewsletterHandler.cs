using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organi.Server.Application.Common.Interfaces;

namespace Organi.Server.Application.Features.Newsletter.Commands.UnsubscribeNewsletter;

public sealed class UnsubscribeNewsletterHandler(
    IApplicationDbContext context,
    ILogger<UnsubscribeNewsletterHandler> logger) : IRequestHandler<UnsubscribeNewsletterCommand>
{
    public async Task Handle(UnsubscribeNewsletterCommand request, CancellationToken cancellationToken)
    {
        var subscriber = await context.NewsletterSubscribers
            .FirstOrDefaultAsync(n => n.Email == request.Email, cancellationToken);

        if (subscriber is null || !subscriber.IsActive)
            return;

        subscriber.IsActive = false;
        subscriber.UnsubscribedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Email {Email} unsubscribed from the newsletter", subscriber.Email);
    }
}
