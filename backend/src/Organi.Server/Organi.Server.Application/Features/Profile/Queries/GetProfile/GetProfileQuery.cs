using MediatR;
using Organi.Server.Application.Features.Profile.DTOs;

namespace Organi.Server.Application.Features.Profile.Queries.GetProfile;

public sealed record GetProfileQuery : IRequest<ProfileResponse>;
