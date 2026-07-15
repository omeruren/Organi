using MediatR;
using Organi.Server.Application.Features.Compare.Commands.AddCompareItem;
using Organi.Server.Application.Features.Compare.Commands.RemoveCompareItem;
using Organi.Server.Application.Features.Compare.DTOs;
using Organi.Server.Application.Features.Compare.Queries.GetCompareList;

namespace Organi.Server.WebAPI.Endpoints;

public static class CompareEndpoints
{
    public static void MapCompareEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/compare").WithTags("Compare").RequireAuthorization();

        group.MapGet("/", GetCompareList)
            .WithName("GetCompareList")
            .WithDescription("Retrieves the current user's product comparison list.")
            .Produces<IReadOnlyList<CompareItemResponse>>(StatusCodes.Status200OK);

        group.MapPost("/", AddCompareItem)
            .WithName("AddCompareItem")
            .WithDescription("Adds a product to the current user's comparison list. Idempotent — returns the existing entry if already present.")
            .Produces<CompareItemResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapDelete("/{productId:guid}", RemoveCompareItem)
            .WithName("RemoveCompareItem")
            .WithDescription("Removes a product from the current user's comparison list.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> GetCompareList(ISender sender, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetCompareListQuery(), cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> AddCompareItem(
        ISender sender,
        AddCompareItemCommand command,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        return Results.Created("/api/compare", result);
    }

    private static async Task<IResult> RemoveCompareItem(
        Guid productId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        await sender.Send(new RemoveCompareItemCommand(productId), cancellationToken);
        return Results.NoContent();
    }
}
