using MediatR;
using Organi.Server.Application.Features.Users.DTOs;

namespace Organi.Server.Application.Features.Users.Commands.DeactivateUser;

public sealed record DeactivateUserCommand(Guid Id) : IRequest<UserResponse>;
