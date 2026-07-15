using FluentValidation;

namespace Organi.Server.Application.Features.Reports.Queries.GetSalesReport;

public sealed class GetSalesReportValidator : AbstractValidator<GetSalesReportQuery>
{
    public GetSalesReportValidator()
    {
        RuleFor(x => x.ToDate)
            .GreaterThanOrEqualTo(x => x.FromDate)
            .When(x => x.FromDate.HasValue && x.ToDate.HasValue)
            .WithMessage("ToDate must be on or after FromDate.");
    }
}
