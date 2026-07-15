using MediatR;
using Organi.Server.Application.Common.Models;
using Organi.Server.Application.Features.Users.Commands.ActivateUser;
using Organi.Server.Application.Features.Users.Commands.AssignUserRoles;
using Organi.Server.Application.Features.Users.Commands.DeactivateUser;
using Organi.Server.Application.Features.Users.DTOs;
using Organi.Server.Application.Features.Users.Queries.GetUserById;
using Organi.Server.Application.Features.Users.Queries.GetUsers;

namespace Organi.Server.WebAPI.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users").WithTags("Users").RequireAuthorization("IsAdmin");

        group.MapGet("/", GetUsers)
            .WithName("GetUsers")
            .WithDescription("Retrieves a paginated list of all users.")
            .Produces<PagedResponse<UserResponse>>(StatusCodes.Status200OK)
            .ProducesValidationProblem();

        group.MapGet("/{id:guid}", GetUserById)
            .WithName("GetUserById")
            .WithDescription("Retrieves a user by their unique identifier.")
            .Produces<UserResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPut("/{id:guid}/activate", ActivateUser)
            .WithName("ActivateUser")
            .WithDescription("Activates a user account.")
            .Produces<UserResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPut("/{id:guid}/deactivate", DeactivateUser)
            .WithName("DeactivateUser")
            .WithDescription("Deactivates a user account.")
            .Produces<UserResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPut("/{id:guid}/roles", AssignUserRoles)
            .WithName("AssignUserRoles")
            .WithDescription("Replaces the full set of roles assigned to a user.")
            .Produces<UserResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }

    private static async Task<IResult> GetUsers(
        ISender sender,
        CancellationToken cancellationToken,
        string? search = null,
        bool? isActive = null,
        int page = 1,
        int pageSize = 10)
    {
        var result = await sender.Send(new GetUsersQuery(search, isActive, page, pageSize), cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetUserById(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetUserByIdQuery(id), cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> ActivateUser(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ActivateUserCommand(id), cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> DeactivateUser(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeactivateUserCommand(id), cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> AssignUserRoles(
        Guid id,
        ISender sender,
        AssignUserRolesCommand command,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command with { Id = id }, cancellationToken);
        return Results.Ok(result);
    }
}
