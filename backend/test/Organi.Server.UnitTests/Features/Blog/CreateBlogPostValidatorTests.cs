using FluentAssertions;
using Organi.Server.Application.Features.Blog.Commands.CreateBlogPost;
using Xunit;

namespace Organi.Server.UnitTests.Features.Blog;

public sealed class CreateBlogPostValidatorTests
{
    private readonly CreateBlogPostValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_ReturnsSuccess()
    {
        var result = _validator.Validate(new CreateBlogPostCommand("Title", "Content", "Excerpt", null, true));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_EmptyTitle_ReturnsError()
    {
        var result = _validator.Validate(new CreateBlogPostCommand("", "Content", null, null, true));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title");
    }

    [Fact]
    public void Validate_EmptyContent_ReturnsError()
    {
        var result = _validator.Validate(new CreateBlogPostCommand("Title", "", null, null, true));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Content");
    }

    [Fact]
    public void Validate_ContentTooLong_ReturnsError()
    {
        var result = _validator.Validate(new CreateBlogPostCommand("Title", new string('a', 10001), null, null, true));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Content");
    }
}
