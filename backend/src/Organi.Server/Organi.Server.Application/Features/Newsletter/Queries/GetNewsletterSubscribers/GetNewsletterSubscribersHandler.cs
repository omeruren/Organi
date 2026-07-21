using MediatR;
using Microsoft.EntityFrameworkCore;
using Organi.Server.Application.Common.Extensions;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Common.Models;
using Organi.Server.Application.Features.Newsletter.DTOs;

namespace Organi.Server.Application.Features.Newsletter.Queries.GetNewsletterSubscribers;

public sealed class GetNewsletterSubscribersHandler(
    IApplicationDbContext context) : IRequestHandler<GetNewsletterSubscribersQuery, PagedResponse<NewsletterSubscriberResponse>>
{
    public async Task<PagedResponse<NewsletterSubscriberResponse>> Handle(GetNewsletterSubscribersQuery request, CancellationToken cancellationToken)
    {
        var query = context.NewsletterSubscribers.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(n => n.Email.Contains(request.Search));

        if (request.IsActive.HasValue)
            query = query.Where(n => n.IsActive == request.IsActive.Value);

        var projected = query
            .OrderByDescending(n => n.SubscribedAt)
            .Select(n => new NewsletterSubscriberResponse(
                n.Id,
                n.Email,
                n.IsActive,
                n.SubscribedAt,
                n.UnsubscribedAt));

        return await projected.ToPagedResponseAsync(request.Page, request.PageSize, cancellationToken);
    }
}
