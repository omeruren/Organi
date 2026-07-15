using MediatR;
using Organi.Server.Application.Features.Users.DTOs;

namespace Organi.Server.Application.Features.Users.Queries.GetUserById;

public sealed record GetUserByIdQuery(Guid Id) : IRequest<UserResponse>;
