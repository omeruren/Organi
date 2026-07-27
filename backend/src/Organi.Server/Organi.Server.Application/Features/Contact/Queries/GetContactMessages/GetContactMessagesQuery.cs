using MediatR;
using Organi.Server.Application.Common.Models;
using Organi.Server.Application.Features.Contact.DTOs;

namespace Organi.Server.Application.Features.Contact.Queries.GetContactMessages;

public sealed record GetContactMessagesQuery(string? Search = null, bool? IsHandled = null, int Page = 1, int PageSize = 10)
    : IRequest<PagedResponse<ContactMessageResponse>>;
