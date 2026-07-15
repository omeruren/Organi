using MediatR;
using Microsoft.EntityFrameworkCore;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Profile.DTOs;
using Organi.Server.Application.Features.Profile.Mappings;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Profile.Queries.GetProfile;

public sealed class GetProfileHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser) : IRequestHandler<GetProfileQuery, ProfileResponse>
{
    public async Task<ProfileResponse> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!.Value;

        var user = await context.Users
            .AsNoTracking()
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
            ?? throw new NotFoundException("User", userId);

        return user.ToResponse();
    }
}
