using MediatR;

namespace Organi.Server.Application.Features.Auth.Commands.ChangePassword;

public sealed record ChangePasswordCommand(string CurrentPassword, string NewPassword) : IRequest;
