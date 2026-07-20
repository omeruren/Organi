using MediatR;
using Organi.Server.Application.Common.Models;
using Organi.Server.Application.Features.Products.DTOs;
using Organi.Server.Application.Features.Products.Queries.GetProducts;
using Organi.Server.Application.Features.Vendors.Commands.ApproveVendor;
using Organi.Server.Application.Features.Vendors.Commands.RegisterVendor;
using Organi.Server.Application.Features.Vendors.Commands.SuspendVendor;
using Organi.Server.Application.Features.Vendors.Commands.UpdateVendor;
using Organi.Server.Application.Features.Orders.DTOs;
using Organi.Server.Application.Features.Vendors.DTOs;
using Organi.Server.Application.Features.Vendors.Queries.GetVendorById;
using Organi.Server.Application.Features.Vendors.Queries.GetVendorDashboardOrders;
using Organi.Server.Application.Features.Vendors.Queries.GetVendorDashboardProducts;
using Organi.Server.Application.Features.Vendors.Queries.GetVendors;

namespace Organi.Server.WebAPI.Endpoints;

public static class VendorEndpoints
{
    public static void MapVendorEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/vendors").WithTags("Vendors");

        group.MapGet("/", GetVendors)
            .WithName("GetVendors")
            .WithDescription("Retrieves a paginated list of approved vendors.")
            .Produces<PagedResponse<VendorResponse>>(StatusCodes.Status200OK)
            .ProducesValidationProblem();

        group.MapGet("/dashboard/products", GetVendorDashboardProducts)
            .WithName("GetVendorDashboardProducts")
            .WithDescription("Retrieves the calling vendor's own products, regardless of status.")
            .RequireAuthorization("IsVendor")
            .Produces<PagedResponse<ProductSummaryResponse>>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status403Forbidden);

        group.MapGet("/dashboard/orders", GetVendorDashboardOrders)
            .WithName("GetVendorDashboardOrders")
            .WithDescription("Retrieves orders containing at least one of the calling vendor's items.")
            .RequireAuthorization("IsVendor")
            .Produces<PagedResponse<OrderSummaryResponse>>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status403Forbidden);

        group.MapGet("/{id:guid}", GetVendorById)
            .WithName("GetVendorById")
            .WithDescription("Retrieves a vendor by its unique identifier.")
            .Produces<VendorResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/{id:guid}/products", GetVendorProducts)
            .WithName("GetVendorProducts")
            .WithDescription("Retrieves a paginated list of a vendor's products.")
            .Produces<PagedResponse<ProductSummaryResponse>>(StatusCodes.Status200OK)
            .ProducesValidationProblem();

        group.MapPost("/register", RegisterVendor)
            .WithName("RegisterVendor")
            .WithDescription("Registers the current user as a pending vendor and grants the Vendor role. Call /api/auth/refresh afterward to obtain a token reflecting the new role.")
            .RequireAuthorization()
            .Produces<VendorResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPut("/{id:guid}", UpdateVendor)
            .WithName("UpdateVendor")
            .WithDescription("Updates a vendor's profile.")
            .RequireAuthorization()
            .Produces<VendorResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPost("/{id:guid}/approve", ApproveVendor)
            .WithName("ApproveVendor")
            .WithDescription("Approves a pending vendor.")
            .RequireAuthorization("CanManageVendors")
            .Produces<VendorResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPost("/{id:guid}/suspend", SuspendVendor)
            .WithName("SuspendVendor")
            .WithDescription("Suspends a vendor.")
            .RequireAuthorization("CanManageVendors")
            .Produces<VendorResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }

    private static async Task<IResult> GetVendors(
        ISender sender,
        CancellationToken cancellationToken,
        string? status = null,
        string? search = null,
        int page = 1,
        int pageSize = 10)
    {
        var result = await sender.Send(new GetVendorsQuery(status, search, page, pageSize), cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetVendorDashboardProducts(
        ISender sender,
        CancellationToken cancellationToken,
        int page = 1,
        int pageSize = 10)
    {
        var result = await sender.Send(new GetVendorDashboardProductsQuery(page, pageSize), cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetVendorDashboardOrders(
        ISender sender,
        CancellationToken cancellationToken,
        string? status = null,
        string? search = null,
        int page = 1,
        int pageSize = 10)
    {
        var result = await sender.Send(new GetVendorDashboardOrdersQuery(status, search, page, pageSize), cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetVendorById(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetVendorByIdQuery(id), cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetVendorProducts(
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
        var query = new GetProductsQuery(null, id, minPrice, maxPrice, isOrganic, null, null, sortBy, sortOrder, page, pageSize);
        var result = await sender.Send(query, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> RegisterVendor(
        ISender sender,
        RegisterVendorCommand command,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        return Results.Created($"/api/vendors/{result.Id}", result);
    }

    private static async Task<IResult> UpdateVendor(
        Guid id,
        ISender sender,
        UpdateVendorCommand command,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command with { Id = id }, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> ApproveVendor(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ApproveVendorCommand(id), cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> SuspendVendor(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new SuspendVendorCommand(id), cancellationToken);
        return Results.Ok(result);
    }
}
