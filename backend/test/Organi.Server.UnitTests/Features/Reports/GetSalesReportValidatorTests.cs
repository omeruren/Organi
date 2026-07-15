using FluentAssertions;
using Organi.Server.Application.Features.Reports.Queries.GetSalesReport;
using Xunit;

namespace Organi.Server.UnitTests.Features.Reports;

public sealed class GetSalesReportValidatorTests
{
    private readonly GetSalesReportValidator _validator = new();

    [Fact]
    public void Validate_NoDates_ReturnsSuccess()
    {
        var result = _validator.Validate(new GetSalesReportQuery());

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ToDateBeforeFromDate_ReturnsError()
    {
        var result = _validator.Validate(new GetSalesReportQuery(
            FromDate: DateTime.UtcNow, ToDate: DateTime.UtcNow.AddDays(-1)));

        result.IsValid.Should().BeFalse();
    }
}
