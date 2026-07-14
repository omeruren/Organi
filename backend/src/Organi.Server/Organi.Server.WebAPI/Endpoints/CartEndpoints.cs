using MediatR;
using Organi.Server.Application.Features.Cart.Commands.AddCartItem;
using Organi.Server.Application.Features.Cart.Commands.ClearCart;
using Organi.Server.Application.Features.Cart.Commands.RemoveCartItem;
using Organi.Server.Application.Features.Cart.Commands.UpdateCartItem;
using Organi.Server.Application.Features.Cart.DTOs;
using Organi.Server.Application.Features.Cart.Queries.GetCart;

namespace Organi.Server.WebAPI.Endpoints;

public static class CartEndpoints
{
    public static void MapCartEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/cart").WithTags("Cart").RequireAuthorization();

        group.MapGet("/", GetCart)
            .WithName("GetCart")
            .WithDescription("Retrieves the current user's cart.")
            .Produces<CartResponse>(StatusCodes.Status200OK);

        group.MapPost("/items", AddCartItem)
            .WithName("AddCartItem")
            .WithDescription("Adds a product to the current user's cart, merging quantity if already present.")
            .Produces<CartResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPut("/items/{id:guid}", UpdateCartItem)
            .WithName("UpdateCartItem")
            .WithDescription("Updates a cart item's quantity.")
            .Produces<CartResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapDelete("/items/{id:guid}", RemoveCartItem)
            .WithName("RemoveCartItem")
            .WithDescription("Removes an item from the cart.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapDelete("/", ClearCart)
            .WithName("ClearCart")
            .WithDescription("Removes all items from the current user's cart.")
            .Produces(StatusCodes.Status204NoContent);
    }

    private static async Task<IResult> GetCart(ISender sender, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetCartQuery(), cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> AddCartItem(
        ISender sender,
        AddCartItemCommand command,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        return Results.Created("/api/cart", result);
    }

    private static async Task<IResult> UpdateCartItem(
        Guid id,
        ISender sender,
        UpdateCartItemCommand command,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command with { Id = id }, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> RemoveCartItem(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        await sender.Send(new RemoveCartItemCommand(id), cancellationToken);
        return Results.NoContent();
    }

    private static async Task<IResult> ClearCart(ISender sender, CancellationToken cancellationToken)
    {
        await sender.Send(new ClearCartCommand(), cancellationToken);
        return Results.NoContent();
    }
}
