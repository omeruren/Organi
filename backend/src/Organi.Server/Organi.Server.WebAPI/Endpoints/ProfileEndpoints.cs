using MediatR;
using Organi.Server.Application.Features.Profile.Commands.UpdateProfile;
using Organi.Server.Application.Features.Profile.DTOs;
using Organi.Server.Application.Features.Profile.Queries.GetProfile;

namespace Organi.Server.WebAPI.Endpoints;

public static class ProfileEndpoints
{
    public static void MapProfileEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/profile").WithTags("Profile").RequireAuthorization();

        group.MapGet("/", GetProfile)
            .WithName("GetProfile")
            .WithDescription("Retrieves the current user's profile.")
            .Produces<ProfileResponse>(StatusCodes.Status200OK);

        group.MapPut("/", UpdateProfile)
            .WithName("UpdateProfile")
            .WithDescription("Updates the current user's profile.")
            .Produces<ProfileResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem();
    }

    private static async Task<IResult> GetProfile(ISender sender, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetProfileQuery(), cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> UpdateProfile(
        ISender sender,
        UpdateProfileCommand command,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        return Results.Ok(result);
    }
}
