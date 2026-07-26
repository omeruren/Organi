using MediatR;

namespace Organi.Server.Application.Features.Auth.Commands.ResetPassword;

public sealed record ResetPasswordCommand(string Email, string Code, string NewPassword) : IRequest;
