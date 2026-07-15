using MediatR;
using Organi.Server.Application.Features.Reports.DTOs;
using Organi.Server.Application.Features.Reports.Queries.GetOrdersReport;
using Organi.Server.Application.Features.Reports.Queries.GetSalesReport;
using Organi.Server.Application.Features.Reports.Queries.GetVendorsReport;

namespace Organi.Server.WebAPI.Endpoints;

public static class ReportEndpoints
{
    public static void MapReportEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/reports").WithTags("Reports").RequireAuthorization("CanViewReports");

        group.MapGet("/sales", GetSalesReport)
            .WithName("GetSalesReport")
            .WithDescription("Retrieves aggregate revenue and order totals for a date range (defaults to all time). Only Confirmed, Processing, Shipped, or Delivered orders count as sales.")
            .Produces<SalesReportResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem();

        group.MapGet("/orders", GetOrdersReport)
            .WithName("GetOrdersReport")
            .WithDescription("Retrieves order counts grouped by status.")
            .Produces<OrdersReportResponse>(StatusCodes.Status200OK);

        group.MapGet("/vendors", GetVendorsReport)
            .WithName("GetVendorsReport")
            .WithDescription("Retrieves the top vendors by revenue.")
            .Produces<IReadOnlyList<VendorReportItemResponse>>(StatusCodes.Status200OK)
            .ProducesValidationProblem();
    }

    private static async Task<IResult> GetSalesReport(
        ISender sender,
        CancellationToken cancellationToken,
        DateTime? fromDate = null,
        DateTime? toDate = null)
    {
        var result = await sender.Send(new GetSalesReportQuery(fromDate, toDate), cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetOrdersReport(ISender sender, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetOrdersReportQuery(), cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetVendorsReport(
        ISender sender,
        CancellationToken cancellationToken,
        int top = 10)
    {
        var result = await sender.Send(new GetVendorsReportQuery(top), cancellationToken);
        return Results.Ok(result);
    }
}
