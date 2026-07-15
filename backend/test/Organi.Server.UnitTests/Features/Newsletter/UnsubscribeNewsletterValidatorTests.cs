using FluentAssertions;
using Organi.Server.Application.Features.Newsletter.Commands.UnsubscribeNewsletter;
using Xunit;

namespace Organi.Server.UnitTests.Features.Newsletter;

public sealed class UnsubscribeNewsletterValidatorTests
{
    private readonly UnsubscribeNewsletterValidator _validator = new();

    [Fact]
    public void Validate_ValidEmail_ReturnsSuccess()
    {
        var result = _validator.Validate(new UnsubscribeNewsletterCommand("someone@organi.test"));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_EmptyEmail_ReturnsError()
    {
        var result = _validator.Validate(new UnsubscribeNewsletterCommand(""));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }
}
