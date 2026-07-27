using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Contact.DTOs;
using Organi.Server.Application.Features.Contact.Mappings;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Contact.Commands.SetContactMessageHandled;

public sealed class SetContactMessageHandledHandler(
    IApplicationDbContext context,
    ILogger<SetContactMessageHandledHandler> logger) : IRequestHandler<SetContactMessageHandledCommand, ContactMessageResponse>
{
    public async Task<ContactMessageResponse> Handle(SetContactMessageHandledCommand request, CancellationToken cancellationToken)
    {
        var message = await context.ContactMessages.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("ContactMessage", request.Id);

        message.IsHandled = request.IsHandled;
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Contact message {MessageId} marked handled={IsHandled}", message.Id, request.IsHandled);

        return message.ToResponse();
    }
}
