using MediatR;
using Organi.Server.Application.Common.Models;
using Organi.Server.Application.Features.Categories.Commands.CreateCategory;
using Organi.Server.Application.Features.Categories.Commands.DeleteCategory;
using Organi.Server.Application.Features.Categories.Commands.UpdateCategory;
using Organi.Server.Application.Features.Categories.DTOs;
using Organi.Server.Application.Features.Categories.Queries.GetCategories;
using Organi.Server.Application.Features.Categories.Queries.GetCategoryById;
using Organi.Server.Application.Features.Products.DTOs;
using Organi.Server.Application.Features.Products.Queries.GetProducts;

namespace Organi.Server.WebAPI.Endpoints;

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/categories").WithTags("Categories");

        group.MapGet("/", GetCategories)
            .WithName("GetCategories")
            .WithDescription("Retrieves the category tree.")
            .Produces<IReadOnlyList<CategoryResponse>>(StatusCodes.Status200OK);

        group.MapGet("/{id:guid}", GetCategoryById)
            .WithName("GetCategoryById")
            .WithDescription("Retrieves a category by its unique identifier.")
            .Produces<CategoryResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/{id:guid}/products", GetProductsByCategory)
            .WithName("GetProductsByCategory")
            .WithDescription("Retrieves a paginated list of products within a category.")
            .Produces<PagedResponse<ProductSummaryResponse>>(StatusCodes.Status200OK)
            .ProducesValidationProblem();

        group.MapPost("/", CreateCategory)
            .WithName("CreateCategory")
            .WithDescription("Creates a new category.")
            .RequireAuthorization("CanManageCategories")
            .Produces<CategoryResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPut("/{id:guid}", UpdateCategory)
            .WithName("UpdateCategory")
            .WithDescription("Updates an existing category.")
            .RequireAuthorization("CanManageCategories")
            .Produces<CategoryResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapDelete("/{id:guid}", DeleteCategory)
            .WithName("DeleteCategory")
            .WithDescription("Soft-deletes a category.")
            .RequireAuthorization("CanManageCategories")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }

    private static async Task<IResult> GetCategories(ISender sender, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetCategoriesQuery(), cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetCategoryById(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetCategoryByIdQuery(id), cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetProductsByCategory(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        bool? isOrganic = null,
        string? sortBy = null,
        string? sortOrder = null,
        int page = 1,
        int pageSize = 10)
    {
        var query = new GetProductsQuery(id, null, minPrice, maxPrice, isOrganic, null, null, sortBy, sortOrder, page, pageSize);
        var result = await sender.Send(query, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> CreateCategory(
        ISender sender,
        CreateCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        return Results.Created($"/api/categories/{result.Id}", result);
    }

    private static async Task<IResult> UpdateCategory(
        Guid id,
        ISender sender,
        UpdateCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command with { Id = id }, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> DeleteCategory(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteCategoryCommand(id), cancellationToken);
        return Results.NoContent();
    }
}
