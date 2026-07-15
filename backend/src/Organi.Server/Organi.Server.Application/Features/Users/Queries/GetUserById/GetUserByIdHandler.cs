using MediatR;
using Microsoft.EntityFrameworkCore;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Users.DTOs;
using Organi.Server.Application.Features.Users.Mappings;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Users.Queries.GetUserById;

public sealed class GetUserByIdHandler(
    IApplicationDbContext context) : IRequestHandler<GetUserByIdQuery, UserResponse>
{
    public async Task<UserResponse> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .AsNoTracking()
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("User", request.Id);

        return user.ToResponse();
    }
}
