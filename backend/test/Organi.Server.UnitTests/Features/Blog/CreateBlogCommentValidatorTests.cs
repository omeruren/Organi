using FluentAssertions;
using Organi.Server.Application.Features.Blog.Commands.CreateBlogComment;
using Xunit;

namespace Organi.Server.UnitTests.Features.Blog;

public sealed class CreateBlogCommentValidatorTests
{
    private readonly CreateBlogCommentValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_ReturnsSuccess()
    {
        var result = _validator.Validate(new CreateBlogCommentCommand("Nice post!"));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_EmptyContent_ReturnsError()
    {
        var result = _validator.Validate(new CreateBlogCommentCommand(""));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Content");
    }

    [Fact]
    public void Validate_ContentTooLong_ReturnsError()
    {
        var result = _validator.Validate(new CreateBlogCommentCommand(new string('a', 1001)));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Content");
    }
}
