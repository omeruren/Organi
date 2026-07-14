using MediatR;
using Organi.Server.Application.Common.Models;
using Organi.Server.Application.Features.Coupons.Commands.CreateCoupon;
using Organi.Server.Application.Features.Coupons.Commands.DeleteCoupon;
using Organi.Server.Application.Features.Coupons.Commands.UpdateCoupon;
using Organi.Server.Application.Features.Coupons.DTOs;
using Organi.Server.Application.Features.Coupons.Queries.GetCouponById;
using Organi.Server.Application.Features.Coupons.Queries.GetCoupons;
using Organi.Server.Application.Features.Coupons.Queries.ValidateCoupon;

namespace Organi.Server.WebAPI.Endpoints;

public static class CouponEndpoints
{
    public static void MapCouponEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/coupons").WithTags("Coupons");

        group.MapGet("/", GetCoupons)
            .WithName("GetCoupons")
            .WithDescription("Retrieves a paginated list of all coupons.")
            .RequireAuthorization("IsAdmin")
            .Produces<PagedResponse<CouponResponse>>(StatusCodes.Status200OK)
            .ProducesValidationProblem();

        group.MapGet("/{id:guid}", GetCouponById)
            .WithName("GetCouponById")
            .WithDescription("Retrieves a coupon by its unique identifier.")
            .RequireAuthorization("IsAdmin")
            .Produces<CouponResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/", CreateCoupon)
            .WithName("CreateCoupon")
            .WithDescription("Creates a new coupon.")
            .RequireAuthorization("IsAdmin")
            .Produces<CouponResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPut("/{id:guid}", UpdateCoupon)
            .WithName("UpdateCoupon")
            .WithDescription("Updates an existing coupon.")
            .RequireAuthorization("IsAdmin")
            .Produces<CouponResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapDelete("/{id:guid}", DeleteCoupon)
            .WithName("DeleteCoupon")
            .WithDescription("Soft-deletes a coupon.")
            .RequireAuthorization("IsAdmin")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/validate", ValidateCoupon)
            .WithName("ValidateCoupon")
            .WithDescription("Validates a coupon code against the current user's cart and previews the discount, without consuming it.")
            .RequireAuthorization()
            .Produces<CouponValidationResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }

    private static async Task<IResult> GetCoupons(
        ISender sender,
        CancellationToken cancellationToken,
        int page = 1,
        int pageSize = 10)
    {
        var result = await sender.Send(new GetCouponsQuery(page, pageSize), cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetCouponById(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetCouponByIdQuery(id), cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> CreateCoupon(
        ISender sender,
        CreateCouponCommand command,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        return Results.Created($"/api/coupons/{result.Id}", result);
    }

    private static async Task<IResult> UpdateCoupon(
        Guid id,
        ISender sender,
        UpdateCouponCommand command,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command with { Id = id }, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> DeleteCoupon(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteCouponCommand(id), cancellationToken);
        return Results.NoContent();
    }

    private static async Task<IResult> ValidateCoupon(
        ISender sender,
        ValidateCouponQuery query,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(query, cancellationToken);
        return Results.Ok(result);
    }
}
