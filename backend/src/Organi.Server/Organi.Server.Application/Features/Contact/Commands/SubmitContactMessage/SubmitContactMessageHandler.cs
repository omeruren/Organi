using MediatR;
using Microsoft.Extensions.Logging;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Contact.DTOs;
using Organi.Server.Application.Features.Contact.Mappings;
using Organi.Server.Domain.Entities;

namespace Organi.Server.Application.Features.Contact.Commands.SubmitContactMessage;

public sealed class SubmitContactMessageHandler(
    IApplicationDbContext context,
    IEmailService emailService,
    ILogger<SubmitContactMessageHandler> logger) : IRequestHandler<SubmitContactMessageCommand, ContactMessageResponse>
{
    public async Task<ContactMessageResponse> Handle(SubmitContactMessageCommand request, CancellationToken cancellationToken)
    {
        var message = new ContactMessage
        {
            Name = request.Name,
            Email = request.Email,
            Subject = request.Subject,
            Message = request.Message,
            IsHandled = false
        };

        context.ContactMessages.Add(message);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Contact message {MessageId} submitted by {Email}", message.Id, message.Email);

        await emailService.SendContactAcknowledgementAsync(
            message.Email, message.Name, message.Subject, CancellationToken.None);

        return message.ToResponse();
    }
}
