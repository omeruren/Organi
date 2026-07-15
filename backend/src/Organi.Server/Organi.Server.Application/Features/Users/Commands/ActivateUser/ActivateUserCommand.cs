using MediatR;
using Organi.Server.Application.Features.Users.DTOs;

namespace Organi.Server.Application.Features.Users.Commands.ActivateUser;

public sealed record ActivateUserCommand(Guid Id) : IRequest<UserResponse>;
