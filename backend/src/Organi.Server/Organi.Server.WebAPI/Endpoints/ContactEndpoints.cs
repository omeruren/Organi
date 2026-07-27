using MediatR;
using Organi.Server.Application.Common.Models;
using Organi.Server.Application.Features.Contact.Commands.SetContactMessageHandled;
using Organi.Server.Application.Features.Contact.Commands.SubmitContactMessage;
using Organi.Server.Application.Features.Contact.DTOs;
using Organi.Server.Application.Features.Contact.Queries.GetContactMessages;

namespace Organi.Server.WebAPI.Endpoints;

public static class ContactEndpoints
{
    public static void MapContactEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/contact").WithTags("Contact");

        group.MapPost("/", SubmitContactMessage)
            .WithName("SubmitContactMessage")
            .WithDescription("Submits a contact message from the storefront contact form.")
            .Produces<ContactMessageResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        group.MapGet("/", GetContactMessages)
            .WithName("GetContactMessages")
            .WithDescription("Retrieves a paginated list of contact messages for admin triage.")
            .RequireAuthorization("IsAdmin")
            .Produces<PagedResponse<ContactMessageResponse>>(StatusCodes.Status200OK)
            .ProducesValidationProblem();

        group.MapPatch("/{id:guid}/handled", SetHandled)
            .WithName("SetContactMessageHandled")
            .WithDescription("Marks a contact message as handled or reopens it.")
            .RequireAuthorization("IsAdmin")
            .Produces<ContactMessageResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> SubmitContactMessage(
        ISender sender,
        SubmitContactMessageCommand command,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        return Results.Created($"/api/contact/{result.Id}", result);
    }

    private static async Task<IResult> GetContactMessages(
        ISender sender,
        CancellationToken cancellationToken,
        string? search = null,
        bool? isHandled = null,
        int page = 1,
        int pageSize = 10)
    {
        var result = await sender.Send(new GetContactMessagesQuery(search, isHandled, page, pageSize), cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> SetHandled(
        Guid id,
        SetContactMessageHandledCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command with { Id = id }, cancellationToken);
        return Results.Ok(result);
    }
}
