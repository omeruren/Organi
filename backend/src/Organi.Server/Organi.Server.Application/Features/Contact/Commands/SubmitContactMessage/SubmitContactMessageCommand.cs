using MediatR;
using Organi.Server.Application.Features.Contact.DTOs;

namespace Organi.Server.Application.Features.Contact.Commands.SubmitContactMessage;

public sealed record SubmitContactMessageCommand(
    string Name,
    string Email,
    string Subject,
    string Message) : IRequest<ContactMessageResponse>;
