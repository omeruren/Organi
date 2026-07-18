using MediatR;
using Organi.Server.Application.Common.Models;
using Organi.Server.Application.Features.Reviews.Commands.CreateReview;
using Organi.Server.Application.Features.Reviews.Commands.DeleteReview;
using Organi.Server.Application.Features.Reviews.Commands.UpdateReview;
using Organi.Server.Application.Features.Reviews.DTOs;
using Organi.Server.Application.Features.Reviews.Queries.GetReviews;
using Organi.Server.Application.Features.Reviews.Queries.GetReviewsByProduct;

namespace Organi.Server.WebAPI.Endpoints;

public static class ReviewEndpoints
{
    public static void MapReviewEndpoints(this IEndpointRouteBuilder app)
    {
        var productReviews = app.MapGroup("/api/products/{productId:guid}/reviews").WithTags("Reviews");

        productReviews.MapGet("/", GetReviewsByProduct)
            .WithName("GetReviewsByProduct")
            .WithDescription("Retrieves a paginated list of approved reviews for a product.")
            .Produces<PagedResponse<ReviewResponse>>(StatusCodes.Status200OK)
            .ProducesValidationProblem();

        productReviews.MapPost("/", CreateReview)
            .WithName("CreateReview")
            .WithDescription("Creates a review for a product. Only customers who purchased the product may review it.")
            .RequireAuthorization()
            .Produces<ReviewResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        var reviews = app.MapGroup("/api/reviews").WithTags("Reviews");

        reviews.MapGet("/", GetReviews)
            .WithName("GetReviews")
            .WithDescription("Retrieves a paginated, filterable list of all reviews for admin moderation.")
            .RequireAuthorization("IsAdmin")
            .Produces<PagedResponse<AdminReviewResponse>>(StatusCodes.Status200OK)
            .ProducesValidationProblem();

        reviews.MapPut("/{id:guid}", UpdateReview)
            .WithName("UpdateReview")
            .WithDescription("Updates an existing review.")
            .RequireAuthorization()
            .Produces<ReviewResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);

        reviews.MapDelete("/{id:guid}", DeleteReview)
            .WithName("DeleteReview")
            .WithDescription("Deletes a review.")
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> GetReviewsByProduct(
        Guid productId,
        ISender sender,
        CancellationToken cancellationToken,
        int page = 1,
        int pageSize = 10)
    {
        var result = await sender.Send(new GetReviewsByProductQuery(productId, page, pageSize), cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> CreateReview(
        Guid productId,
        ISender sender,
        CreateReviewCommand command,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command with { ProductId = productId }, cancellationToken);
        return Results.Created($"/api/reviews/{result.Id}", result);
    }

    private static async Task<IResult> GetReviews(
        ISender sender,
        CancellationToken cancellationToken,
        Guid? productId = null,
        int? rating = null,
        int page = 1,
        int pageSize = 10)
    {
        var result = await sender.Send(new GetReviewsQuery(productId, rating, page, pageSize), cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> UpdateReview(
        Guid id,
        ISender sender,
        UpdateReviewCommand command,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command with { Id = id }, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> DeleteReview(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteReviewCommand(id), cancellationToken);
        return Results.NoContent();
    }
}
