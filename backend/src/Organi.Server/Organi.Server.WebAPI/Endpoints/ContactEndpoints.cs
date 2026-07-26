using MediatR;
using Organi.Server.Application.Features.Contact.Commands.SubmitContactMessage;
using Organi.Server.Application.Features.Contact.DTOs;

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
    }

    private static async Task<IResult> SubmitContactMessage(
        ISender sender,
        SubmitContactMessageCommand command,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        return Results.Created($"/api/contact/{result.Id}", result);
    }
}
