using FluentValidation;

namespace Organi.Server.Application.Features.Reports.Queries.GetVendorsReport;

public sealed class GetVendorsReportValidator : AbstractValidator<GetVendorsReportQuery>
{
    public GetVendorsReportValidator()
    {
        RuleFor(x => x.Top)
            .InclusiveBetween(1, 100).WithMessage("Top must be between 1 and 100.");
    }
}
