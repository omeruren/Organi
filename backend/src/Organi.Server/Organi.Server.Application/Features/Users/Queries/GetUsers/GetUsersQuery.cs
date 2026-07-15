using MediatR;
using Organi.Server.Application.Common.Models;
using Organi.Server.Application.Features.Users.DTOs;

namespace Organi.Server.Application.Features.Users.Queries.GetUsers;

public sealed record GetUsersQuery(string? Search = null, bool? IsActive = null, int Page = 1, int PageSize = 10)
    : IRequest<PagedResponse<UserResponse>>;
