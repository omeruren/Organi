using Organi.Server.Application.Features.Newsletter.DTOs;
using Organi.Server.Domain.Entities;

namespace Organi.Server.Application.Features.Newsletter.Mappings;

public static class NewsletterMappingExtensions
{
    public static NewsletterSubscriberResponse ToResponse(this NewsletterSubscriber subscriber) => new(
        subscriber.Id,
        subscriber.Email,
        subscriber.IsActive,
        subscriber.SubscribedAt,
        subscriber.UnsubscribedAt);
}
