using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Profile.DTOs;
using Organi.Server.Application.Features.Profile.Mappings;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Profile.Commands.UpdateProfile;

public sealed class UpdateProfileHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser,
    ILogger<UpdateProfileHandler> logger) : IRequestHandler<UpdateProfileCommand, ProfileResponse>
{
    public async Task<ProfileResponse> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!.Value;

        var user = await context.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
            ?? throw new NotFoundException("User", userId);

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.PhoneNumber = request.PhoneNumber;
        user.DateOfBirth = request.DateOfBirth;
        user.AvatarUrl = request.AvatarUrl;

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("User {UserId} updated their profile", userId);

        return user.ToResponse();
    }
}
