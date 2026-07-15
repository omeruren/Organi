using FluentValidation;

namespace Organi.Server.Application.Features.AuditLogs.Queries.GetAuditLogs;

public sealed class GetAuditLogsValidator : AbstractValidator<GetAuditLogsQuery>
{
    public GetAuditLogsValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page must be at least 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 50).WithMessage("Page size must be between 1 and 50.");

        RuleFor(x => x.ToDate)
            .GreaterThanOrEqualTo(x => x.FromDate)
            .When(x => x.FromDate.HasValue && x.ToDate.HasValue)
            .WithMessage("ToDate must be on or after FromDate.");
    }
}
