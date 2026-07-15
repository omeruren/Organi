using FluentAssertions;
using Organi.Server.Application.Features.Newsletter.Commands.SubscribeNewsletter;
using Xunit;

namespace Organi.Server.UnitTests.Features.Newsletter;

public sealed class SubscribeNewsletterValidatorTests
{
    private readonly SubscribeNewsletterValidator _validator = new();

    [Fact]
    public void Validate_ValidEmail_ReturnsSuccess()
    {
        var result = _validator.Validate(new SubscribeNewsletterCommand("someone@organi.test"));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_EmptyEmail_ReturnsError()
    {
        var result = _validator.Validate(new SubscribeNewsletterCommand(""));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public void Validate_MalformedEmail_ReturnsError()
    {
        var result = _validator.Validate(new SubscribeNewsletterCommand("not-an-email"));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }
}
