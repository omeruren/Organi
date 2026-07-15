using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Blog.Commands.CreateBlogComment;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Exceptions;
using Xunit;

namespace Organi.Server.UnitTests.Features.Blog;

public sealed class CreateBlogCommentHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly ILogger<CreateBlogCommentHandler> _logger = Substitute.For<ILogger<CreateBlogCommentHandler>>();
    private readonly CreateBlogCommentHandler _handler;

    public CreateBlogCommentHandlerTests()
    {
        _handler = new CreateBlogCommentHandler(_context, _currentUser, _logger);
    }

    private void SetupBlogPosts(params BlogPost[] posts)
    {
        var mockSet = posts.ToList().BuildMockDbSet();
        _context.BlogPosts.Returns(mockSet);
    }

    private void SetupUsers(params User[] users)
    {
        var mockSet = users.ToList().BuildMockDbSet();
        _context.Users.Returns(mockSet);
    }

    [Fact]
    public async Task Handle_PublishedPost_CreatesComment()
    {
        var post = new BlogPost { Title = "Title", Slug = "title", Content = "Content", IsPublished = true };
        var user = new User { Email = "commenter@organi.test", FirstName = "Ada", LastName = "Lovelace" };
        SetupBlogPosts(post);
        SetupUsers(user);
        _currentUser.UserId.Returns(user.Id);

        var result = await _handler.Handle(
            new CreateBlogCommentCommand("Nice post!") { BlogPostId = post.Id }, CancellationToken.None);

        result.Content.Should().Be("Nice post!");
    }

    [Fact]
    public async Task Handle_UnpublishedPost_ThrowsBusinessRuleException()
    {
        var post = new BlogPost { Title = "Title", Slug = "title", Content = "Content", IsPublished = false };
        var user = new User { Email = "commenter@organi.test", FirstName = "Ada", LastName = "Lovelace" };
        SetupBlogPosts(post);
        SetupUsers(user);
        _currentUser.UserId.Returns(user.Id);

        var act = () => _handler.Handle(
            new CreateBlogCommentCommand("Nice post!") { BlogPostId = post.Id }, CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>();
    }

    [Fact]
    public async Task Handle_PostNotFound_ThrowsNotFoundException()
    {
        SetupBlogPosts();
        SetupUsers(new User { Email = "commenter@organi.test", FirstName = "Ada", LastName = "Lovelace" });
        _currentUser.UserId.Returns(Guid.NewGuid());

        var act = () => _handler.Handle(
            new CreateBlogCommentCommand("Nice post!") { BlogPostId = Guid.NewGuid() }, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
