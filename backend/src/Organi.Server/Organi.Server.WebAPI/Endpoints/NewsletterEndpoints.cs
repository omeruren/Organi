using MediatR;
using Organi.Server.Application.Common.Models;
using Organi.Server.Application.Features.Newsletter.Commands.SubscribeNewsletter;
using Organi.Server.Application.Features.Newsletter.Commands.UnsubscribeNewsletter;
using Organi.Server.Application.Features.Newsletter.DTOs;
using Organi.Server.Application.Features.Newsletter.Queries.GetNewsletterSubscribers;

namespace Organi.Server.WebAPI.Endpoints;

public static class NewsletterEndpoints
{
    public static void MapNewsletterEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/newsletter").WithTags("Newsletter");

        group.MapPost("/subscribe", Subscribe)
            .WithName("SubscribeNewsletter")
            .WithDescription("Subscribes an email address to the newsletter. Idempotent — resubscribes if previously unsubscribed.")
            .Produces<NewsletterSubscriberResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem();

        group.MapPost("/unsubscribe", Unsubscribe)
            .WithName("UnsubscribeNewsletter")
            .WithDescription("Unsubscribes an email address from the newsletter. Idempotent — succeeds even if already unsubscribed or unknown.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem();

        group.MapGet("/subscribers", GetSubscribers)
            .WithName("GetNewsletterSubscribers")
            .WithDescription("Retrieves a paginated list of newsletter subscribers.")
            .RequireAuthorization("IsAdmin")
            .Produces<PagedResponse<NewsletterSubscriberResponse>>(StatusCodes.Status200OK)
            .ProducesValidationProblem();
    }

    private static async Task<IResult> Subscribe(
        ISender sender,
        SubscribeNewsletterCommand command,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> Unsubscribe(
        ISender sender,
        UnsubscribeNewsletterCommand command,
        CancellationToken cancellationToken)
    {
        await sender.Send(command, cancellationToken);
        return Results.NoContent();
    }

    private static async Task<IResult> GetSubscribers(
        ISender sender,
        CancellationToken cancellationToken,
        string? search = null,
        bool? isActive = null,
        int page = 1,
        int pageSize = 10)
    {
        var result = await sender.Send(new GetNewsletterSubscribersQuery(search, isActive, page, pageSize), cancellationToken);
        return Results.Ok(result);
    }
}
