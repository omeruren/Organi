using MediatR;
using Organi.Server.Application.Features.Auth.Commands.ChangePassword;
using Organi.Server.Application.Features.Auth.Commands.ConfirmEmail;
using Organi.Server.Application.Features.Auth.Commands.ForgotPassword;
using Organi.Server.Application.Features.Auth.Commands.Login;
using Organi.Server.Application.Features.Auth.Commands.Logout;
using Organi.Server.Application.Features.Auth.Commands.Refresh;
using Organi.Server.Application.Features.Auth.Commands.Register;
using Organi.Server.Application.Features.Auth.Commands.ResendConfirmation;
using Organi.Server.Application.Features.Auth.Commands.ResetPassword;
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

        group.MapPost("/confirm-email", ConfirmEmail)
            .WithName("ConfirmEmail")
            .WithDescription("Confirms an email address from the token in the confirmation link.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPost("/resend-confirmation", ResendConfirmation)
            .WithName("ResendConfirmation")
            .WithDescription("Re-sends the confirmation email. Always succeeds, whether or not the address has an unconfirmed account.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem();

        group.MapPost("/forgot-password", ForgotPassword)
            .WithName("ForgotPassword")
            .WithDescription("Emails a password reset code. Always succeeds, whether or not the address has an account.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem();

        group.MapPost("/reset-password", ResetPassword)
            .WithName("ResetPassword")
            .WithDescription("Sets a new password from an emailed reset code and revokes all sessions.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
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

    private static async Task<IResult> ConfirmEmail(
        ISender sender,
        ConfirmEmailCommand command,
        CancellationToken cancellationToken)
    {
        await sender.Send(command, cancellationToken);
        return Results.NoContent();
    }

    private static async Task<IResult> ResendConfirmation(
        ISender sender,
        ResendConfirmationCommand command,
        CancellationToken cancellationToken)
    {
        await sender.Send(command, cancellationToken);
        return Results.NoContent();
    }

    private static async Task<IResult> ForgotPassword(
        ISender sender,
        ForgotPasswordCommand command,
        CancellationToken cancellationToken)
    {
        await sender.Send(command, cancellationToken);
        return Results.NoContent();
    }

    private static async Task<IResult> ResetPassword(
        ISender sender,
        ResetPasswordCommand command,
        CancellationToken cancellationToken)
    {
        await sender.Send(command, cancellationToken);
        return Results.NoContent();
    }
}
