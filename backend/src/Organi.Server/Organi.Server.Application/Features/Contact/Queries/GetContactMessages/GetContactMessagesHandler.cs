using MediatR;
using Microsoft.EntityFrameworkCore;
using Organi.Server.Application.Common.Extensions;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Common.Models;
using Organi.Server.Application.Features.Contact.DTOs;

namespace Organi.Server.Application.Features.Contact.Queries.GetContactMessages;

public sealed class GetContactMessagesHandler(
    IApplicationDbContext context) : IRequestHandler<GetContactMessagesQuery, PagedResponse<ContactMessageResponse>>
{
    public async Task<PagedResponse<ContactMessageResponse>> Handle(GetContactMessagesQuery request, CancellationToken cancellationToken)
    {
        var query = context.ContactMessages.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            // Admins triage by whoever wrote in or what it was about, so cover all three.
            query = query.Where(c =>
                c.Name.Contains(request.Search) ||
                c.Email.Contains(request.Search) ||
                c.Subject.Contains(request.Search));
        }

        if (request.IsHandled.HasValue)
            query = query.Where(c => c.IsHandled == request.IsHandled.Value);

        var projected = query
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new ContactMessageResponse(
                c.Id,
                c.Name,
                c.Email,
                c.Subject,
                c.Message,
                c.IsHandled,
                c.CreatedAt));

        return await projected.ToPagedResponseAsync(request.Page, request.PageSize, cancellationToken);
    }
}
