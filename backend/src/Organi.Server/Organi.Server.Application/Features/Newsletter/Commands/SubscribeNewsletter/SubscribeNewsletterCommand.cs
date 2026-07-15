using MediatR;
using Organi.Server.Application.Features.Newsletter.DTOs;

namespace Organi.Server.Application.Features.Newsletter.Commands.SubscribeNewsletter;

public sealed record SubscribeNewsletterCommand(string Email) : IRequest<NewsletterSubscriberResponse>;
