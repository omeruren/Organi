using Organi.Server.Application.Features.Contact.DTOs;
using Organi.Server.Domain.Entities;

namespace Organi.Server.Application.Features.Contact.Mappings;

public static class ContactMappingExtensions
{
    public static ContactMessageResponse ToResponse(this ContactMessage message) => new(
        message.Id,
        message.Name,
        message.Email,
        message.Subject,
        message.Message,
        message.IsHandled,
        message.CreatedAt);
}
