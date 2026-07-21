using MediatR;
using Organi.Server.Application.Common.Models;
using Organi.Server.Application.Features.Newsletter.DTOs;

namespace Organi.Server.Application.Features.Newsletter.Queries.GetNewsletterSubscribers;

public sealed record GetNewsletterSubscribersQuery(string? Search = null, bool? IsActive = null, int Page = 1, int PageSize = 10)
    : IRequest<PagedResponse<NewsletterSubscriberResponse>>;
