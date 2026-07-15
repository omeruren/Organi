using MediatR;
using Organi.Server.Application.Features.Profile.DTOs;

namespace Organi.Server.Application.Features.Profile.Commands.UpdateProfile;

public sealed record UpdateProfileCommand(
    string FirstName,
    string LastName,
    string? PhoneNumber,
    DateOnly? DateOfBirth,
    string? AvatarUrl) : IRequest<ProfileResponse>;
