using MediatR;
using Organi.Server.Application.Common.Models;
using Organi.Server.Application.Features.Products.Commands.CreateProduct;
using Organi.Server.Application.Features.Products.Commands.DeleteProduct;
using Organi.Server.Application.Features.Products.Commands.UpdateProduct;
using Organi.Server.Application.Features.Products.DTOs;
using Organi.Server.Application.Features.Products.Queries.GetFeaturedProducts;
using Organi.Server.Application.Features.Products.Queries.GetProductById;
using Organi.Server.Application.Features.Products.Queries.GetProductBySlug;
using Organi.Server.Application.Features.Products.Queries.GetProducts;

namespace Organi.Server.WebAPI.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products").WithTags("Products");

        group.MapGet("/", GetProducts)
            .WithName("GetProducts")
            .WithDescription("Retrieves a paginated, filterable, sortable list of products.")
            .Produces<PagedResponse<ProductSummaryResponse>>(StatusCodes.Status200OK)
            .ProducesValidationProblem();

        group.MapGet("/featured", GetFeaturedProducts)
            .WithName("GetFeaturedProducts")
            .WithDescription("Retrieves a curated list of featured products.")
            .Produces<IReadOnlyList<ProductSummaryResponse>>(StatusCodes.Status200OK)
            .ProducesValidationProblem();

        group.MapGet("/slug/{slug}", GetProductBySlug)
            .WithName("GetProductBySlug")
            .WithDescription("Retrieves a product by its URL-friendly slug.")
            .Produces<ProductResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/{id:guid}", GetProductById)
            .WithName("GetProductById")
            .WithDescription("Retrieves a product by its unique identifier.")
            .Produces<ProductResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/", CreateProduct)
            .WithName("CreateProduct")
            .WithDescription("Creates a new product in the calling vendor's catalog.")
            .RequireAuthorization("CanManageProducts")
            .Produces<ProductResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPut("/{id:guid}", UpdateProduct)
            .WithName("UpdateProduct")
            .WithDescription("Updates an existing product.")
            .RequireAuthorization("CanManageProducts")
            .Produces<ProductResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapDelete("/{id:guid}", DeleteProduct)
            .WithName("DeleteProduct")
            .WithDescription("Soft-deletes a product.")
            .RequireAuthorization("CanManageProducts")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }

    private static async Task<IResult> GetProducts(
        ISender sender,
        CancellationToken cancellationToken,
        Guid? categoryId = null,
        Guid? vendorId = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        bool? isOrganic = null,
        string? status = null,
        string? search = null,
        string? sortBy = null,
        string? sortOrder = null,
        int page = 1,
        int pageSize = 10)
    {
        var query = new GetProductsQuery(categoryId, vendorId, minPrice, maxPrice, isOrganic, status, search, sortBy, sortOrder, page, pageSize);
        var result = await sender.Send(query, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetFeaturedProducts(
        ISender sender,
        CancellationToken cancellationToken,
        int take = 8)
    {
        var result = await sender.Send(new GetFeaturedProductsQuery(take), cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetProductBySlug(
        string slug,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetProductBySlugQuery(slug), cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetProductById(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetProductByIdQuery(id), cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> CreateProduct(
        ISender sender,
        CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        return Results.Created($"/api/products/{result.Id}", result);
    }

    private static async Task<IResult> UpdateProduct(
        Guid id,
        ISender sender,
        UpdateProductCommand command,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command with { Id = id }, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> DeleteProduct(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteProductCommand(id), cancellationToken);
        return Results.NoContent();
    }
}
