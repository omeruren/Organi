using MediatR;
using Organi.Server.Application.Features.Users.DTOs;

namespace Organi.Server.Application.Features.Users.Commands.AssignUserRoles;

public sealed record AssignUserRolesCommand(IReadOnlyList<string> RoleNames) : IRequest<UserResponse>
{
    public Guid Id { get; init; }
}
