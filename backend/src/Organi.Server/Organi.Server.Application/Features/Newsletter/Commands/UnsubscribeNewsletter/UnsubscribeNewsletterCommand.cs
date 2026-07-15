using MediatR;

namespace Organi.Server.Application.Features.Newsletter.Commands.UnsubscribeNewsletter;

public sealed record UnsubscribeNewsletterCommand(string Email) : IRequest;
