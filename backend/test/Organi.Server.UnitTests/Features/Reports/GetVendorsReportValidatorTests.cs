using FluentAssertions;
using Organi.Server.Application.Features.Reports.Queries.GetVendorsReport;
using Xunit;

namespace Organi.Server.UnitTests.Features.Reports;

public sealed class GetVendorsReportValidatorTests
{
    private readonly GetVendorsReportValidator _validator = new();

    [Fact]
    public void Validate_DefaultTop_ReturnsSuccess()
    {
        var result = _validator.Validate(new GetVendorsReportQuery());

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_TopOutOfRange_ReturnsError()
    {
        var result = _validator.Validate(new GetVendorsReportQuery(Top: 0));

        result.IsValid.Should().BeFalse();
    }
}
