using MediatR;
using Organi.Server.Application.Features.Wishlist.Commands.AddWishlistItem;
using Organi.Server.Application.Features.Wishlist.Commands.RemoveWishlistItem;
using Organi.Server.Application.Features.Wishlist.DTOs;
using Organi.Server.Application.Features.Wishlist.Queries.GetWishlist;

namespace Organi.Server.WebAPI.Endpoints;

public static class WishlistEndpoints
{
    public static void MapWishlistEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/wishlist").WithTags("Wishlist").RequireAuthorization();

        group.MapGet("/", GetWishlist)
            .WithName("GetWishlist")
            .WithDescription("Retrieves the current user's wishlist.")
            .Produces<IReadOnlyList<WishlistItemResponse>>(StatusCodes.Status200OK);

        group.MapPost("/", AddWishlistItem)
            .WithName("AddWishlistItem")
            .WithDescription("Adds a product to the current user's wishlist. Idempotent — returns the existing entry if already present.")
            .Produces<WishlistItemResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapDelete("/{productId:guid}", RemoveWishlistItem)
            .WithName("RemoveWishlistItem")
            .WithDescription("Removes a product from the current user's wishlist.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> GetWishlist(ISender sender, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetWishlistQuery(), cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> AddWishlistItem(
        ISender sender,
        AddWishlistItemCommand command,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        return Results.Created("/api/wishlist", result);
    }

    private static async Task<IResult> RemoveWishlistItem(
        Guid productId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        await sender.Send(new RemoveWishlistItemCommand(productId), cancellationToken);
        return Results.NoContent();
    }
}
