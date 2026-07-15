using MediatR;
using Organi.Server.Application.Features.Reports.DTOs;

namespace Organi.Server.Application.Features.Reports.Queries.GetVendorsReport;

public sealed record GetVendorsReportQuery(int Top = 10) : IRequest<IReadOnlyList<VendorReportItemResponse>>;
