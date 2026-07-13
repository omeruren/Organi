using MediatR;
using Organi.Server.Application.Features.Auth.Commands.ChangePassword;
using Organi.Server.Application.Features.Auth.Commands.Login;
using Organi.Server.Application.Features.Auth.Commands.Logout;
using Organi.Server.Application.Features.Auth.Commands.Refresh;
using Organi.Server.Application.Features.Auth.Commands.Register;
using Organi.Server.Application.Features.Auth.DTOs;
using Organi.Server.WebAPI.Extensions;

namespace Organi.Server.WebAPI.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/register", Register)
            .WithName("Register")
            .WithDescription("Registers a new customer account and returns a token pair.")
            .Produces<AuthResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPost("/login", Login)
            .WithName("Login")
            .WithDescription("Authenticates a user and returns a token pair.")
            .Produces<AuthResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        group.MapPost("/refresh", Refresh)
            .WithName("RefreshToken")
            .WithDescription("Rotates a refresh token for a new token pair.")
            .Produces<AuthResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        group.MapPost("/logout", Logout)
            .WithName("Logout")
            .WithDescription("Revokes all refresh tokens for the current user.")
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        group.MapPost("/change-password", ChangePassword)
            .WithName("ChangePassword")
            .WithDescription("Changes the current user's password and revokes all sessions.")
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }

    private static async Task<IResult> Register(
        ISender sender,
        RegisterCommand command,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> Login(
        ISender sender,
        LoginCommand command,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        return result.ToApiResult();
    }

    private static async Task<IResult> Refresh(
        ISender sender,
        RefreshTokenCommand command,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        return result.ToApiResult();
    }

    private static async Task<IResult> Logout(
        ISender sender,
        CancellationToken cancellationToken)
    {
        await sender.Send(new LogoutCommand(), cancellationToken);
        return Results.NoContent();
    }

    private static async Task<IResult> ChangePassword(
        ISender sender,
        ChangePasswordCommand command,
        CancellationToken cancellationToken)
    {
        await sender.Send(command, cancellationToken);
        return Results.NoContent();
    }
}
