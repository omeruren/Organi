using MediatR;

namespace Organi.Server.Application.Features.Auth.Commands.ConfirmEmail;

public sealed record ConfirmEmailCommand(string Token) : IRequest;
