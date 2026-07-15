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
        var projected = context.NewsletterSubscribers
            .AsNoTracking()
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
