using MediatR;
using Microsoft.EntityFrameworkCore;
using Organi.Server.Application.Common.Extensions;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Common.Models;
using Organi.Server.Application.Features.Users.DTOs;

namespace Organi.Server.Application.Features.Users.Queries.GetUsers;

public sealed class GetUsersHandler(
    IApplicationDbContext context) : IRequestHandler<GetUsersQuery, PagedResponse<UserResponse>>
{
    public async Task<PagedResponse<UserResponse>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var query = context.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(u =>
                u.Email.Contains(request.Search) ||
                u.FirstName.Contains(request.Search) ||
                u.LastName.Contains(request.Search));
        }

        if (request.IsActive.HasValue)
            query = query.Where(u => u.IsActive == request.IsActive.Value);

        var projected = query
            .Include(u => u.Roles)
            .OrderByDescending(u => u.CreatedAt)
            .Select(u => new UserResponse(
                u.Id,
                u.Email,
                u.FirstName,
                u.LastName,
                u.PhoneNumber,
                u.IsActive,
                u.EmailConfirmed,
                u.LastLoginAt,
                u.Roles.Select(r => r.Name).ToList(),
                u.CreatedAt));

        return await projected.ToPagedResponseAsync(request.Page, request.PageSize, cancellationToken);
    }
}
