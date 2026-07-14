using MediatR;
using Organi.Server.Application.Common.Models;
using Organi.Server.Application.Features.Orders.Commands.CancelOrder;
using Organi.Server.Application.Features.Orders.Commands.ConfirmOrder;
using Organi.Server.Application.Features.Orders.Commands.CreateOrder;
using Organi.Server.Application.Features.Orders.Commands.DeliverOrder;
using Organi.Server.Application.Features.Orders.Commands.ShipOrder;
using Organi.Server.Application.Features.Orders.DTOs;
using Organi.Server.Application.Features.Orders.Queries.GetMyOrders;
using Organi.Server.Application.Features.Orders.Queries.GetOrderById;
using Organi.Server.Application.Features.Orders.Queries.GetOrders;

namespace Organi.Server.WebAPI.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders").WithTags("Orders");

        group.MapGet("/", GetOrders)
            .WithName("GetOrders")
            .WithDescription("Retrieves a paginated list of all orders.")
            .RequireAuthorization("IsAdmin")
            .Produces<PagedResponse<OrderSummaryResponse>>(StatusCodes.Status200OK)
            .ProducesValidationProblem();

        group.MapGet("/my", GetMyOrders)
            .WithName("GetMyOrders")
            .WithDescription("Retrieves a paginated list of the current user's orders.")
            .RequireAuthorization()
            .Produces<PagedResponse<OrderSummaryResponse>>(StatusCodes.Status200OK)
            .ProducesValidationProblem();

        group.MapGet("/{id:guid}", GetOrderById)
            .WithName("GetOrderById")
            .WithDescription("Retrieves an order by its unique identifier.")
            .RequireAuthorization()
            .Produces<OrderResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/", CreateOrder)
            .WithName("CreateOrder")
            .WithDescription("Creates an order (checkout) from the current user's cart.")
            .RequireAuthorization()
            .Produces<OrderResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPost("/{id:guid}/confirm", ConfirmOrder)
            .WithName("ConfirmOrder")
            .WithDescription("Confirms a pending order and decrements stock.")
            .RequireAuthorization("CanManageOrders")
            .Produces<OrderResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPost("/{id:guid}/ship", ShipOrder)
            .WithName("ShipOrder")
            .WithDescription("Marks a confirmed order as shipped.")
            .RequireAuthorization("CanManageOrders")
            .Produces<OrderResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPost("/{id:guid}/deliver", DeliverOrder)
            .WithName("DeliverOrder")
            .WithDescription("Marks a shipped order as delivered.")
            .RequireAuthorization("IsAdmin")
            .Produces<OrderResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPost("/{id:guid}/cancel", CancelOrder)
            .WithName("CancelOrder")
            .WithDescription("Cancels an order, restoring stock if it had already been confirmed.")
            .RequireAuthorization()
            .Produces<OrderResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }

    private static async Task<IResult> GetOrders(
        ISender sender,
        CancellationToken cancellationToken,
        int page = 1,
        int pageSize = 10)
    {
        var result = await sender.Send(new GetOrdersQuery(page, pageSize), cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetMyOrders(
        ISender sender,
        CancellationToken cancellationToken,
        int page = 1,
        int pageSize = 10)
    {
        var result = await sender.Send(new GetMyOrdersQuery(page, pageSize), cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetOrderById(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetOrderByIdQuery(id), cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> CreateOrder(
        ISender sender,
        CreateOrderCommand command,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        return Results.Created($"/api/orders/{result.Id}", result);
    }

    private static async Task<IResult> ConfirmOrder(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ConfirmOrderCommand(id), cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> ShipOrder(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ShipOrderCommand(id), cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> DeliverOrder(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeliverOrderCommand(id), cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> CancelOrder(
        Guid id,
        ISender sender,
        CancelOrderCommand command,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command with { Id = id }, cancellationToken);
        return Results.Ok(result);
    }
}
