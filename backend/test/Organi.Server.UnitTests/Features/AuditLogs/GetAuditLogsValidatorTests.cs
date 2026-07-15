using FluentAssertions;
using Organi.Server.Application.Features.AuditLogs.Queries.GetAuditLogs;
using Xunit;

namespace Organi.Server.UnitTests.Features.AuditLogs;

public sealed class GetAuditLogsValidatorTests
{
    private readonly GetAuditLogsValidator _validator = new();

    [Fact]
    public void Validate_ValidQuery_ReturnsSuccess()
    {
        var result = _validator.Validate(new GetAuditLogsQuery());

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ToDateBeforeFromDate_ReturnsError()
    {
        var result = _validator.Validate(new GetAuditLogsQuery(
            FromDate: DateTime.UtcNow, ToDate: DateTime.UtcNow.AddDays(-1)));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ToDate");
    }
}
