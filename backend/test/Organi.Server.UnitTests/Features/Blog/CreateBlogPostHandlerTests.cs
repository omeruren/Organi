using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Blog.Commands.CreateBlogPost;
using Organi.Server.Domain.Entities;
using Xunit;

namespace Organi.Server.UnitTests.Features.Blog;

public sealed class CreateBlogPostHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly ILogger<CreateBlogPostHandler> _logger = Substitute.For<ILogger<CreateBlogPostHandler>>();
    private readonly CreateBlogPostHandler _handler;

    public CreateBlogPostHandlerTests()
    {
        _handler = new CreateBlogPostHandler(_context, _currentUser, _logger);
    }

    private void SetupUsers(params User[] users)
    {
        var mockSet = users.ToList().BuildMockDbSet();
        _context.Users.Returns(mockSet);
    }

    private void SetupBlogPosts(params BlogPost[] posts)
    {
        var mockSet = posts.ToList().BuildMockDbSet();
        _context.BlogPosts.Returns(mockSet);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesPublishedPostWithSlugAndTimestamp()
    {
        var author = new User { Email = "author@organi.test", FirstName = "Ada", LastName = "Lovelace" };
        SetupUsers(author);
        SetupBlogPosts();
        _currentUser.UserId.Returns(author.Id);

        var result = await _handler.Handle(
            new CreateBlogPostCommand("Fresh Organic Tips", "Some content", "Excerpt", null, true), CancellationToken.None);

        result.Title.Should().Be("Fresh Organic Tips");
        result.Slug.Should().Be("fresh-organic-tips");
        result.IsPublished.Should().BeTrue();
        result.PublishedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_DraftPost_DoesNotSetPublishedAt()
    {
        var author = new User { Email = "author@organi.test", FirstName = "Ada", LastName = "Lovelace" };
        SetupUsers(author);
        SetupBlogPosts();
        _currentUser.UserId.Returns(author.Id);

        var result = await _handler.Handle(
            new CreateBlogPostCommand("Draft Post", "Some content", null, null, false), CancellationToken.None);

        result.IsPublished.Should().BeFalse();
        result.PublishedAt.Should().BeNull();
    }
}
