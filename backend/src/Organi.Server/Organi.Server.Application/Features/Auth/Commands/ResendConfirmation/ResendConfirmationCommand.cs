using MediatR;

namespace Organi.Server.Application.Features.Auth.Commands.ResendConfirmation;

public sealed record ResendConfirmationCommand(string Email) : IRequest;
