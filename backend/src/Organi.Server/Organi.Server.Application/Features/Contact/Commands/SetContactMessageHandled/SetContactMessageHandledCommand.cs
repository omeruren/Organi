using MediatR;
using Organi.Server.Application.Features.Contact.DTOs;

namespace Organi.Server.Application.Features.Contact.Commands.SetContactMessageHandled;

// Id travels in the URL; IsHandled is the toggle so a message can be reopened as well as closed.
public sealed record SetContactMessageHandledCommand(Guid Id, bool IsHandled) : IRequest<ContactMessageResponse>;
