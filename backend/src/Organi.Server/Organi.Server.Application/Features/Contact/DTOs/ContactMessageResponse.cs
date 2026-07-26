namespace Organi.Server.Application.Features.Contact.DTOs;

public sealed record ContactMessageResponse(
    Guid Id,
    string Name,
    string Email,
    string Subject,
    string Message,
    bool IsHandled,
    DateTime CreatedAt);
