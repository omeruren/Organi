using MediatR;

namespace Organi.Server.Application.Features.Auth.Commands.ForgotPassword;

public sealed record ForgotPasswordCommand(string Email) : IRequest;
