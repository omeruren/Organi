namespace Organi.Server.Application.Features.Newsletter.DTOs;

public sealed record NewsletterSubscriberResponse(
    Guid Id,
    string Email,
    bool IsActive,
    DateTime SubscribedAt,
    DateTime? UnsubscribedAt);
