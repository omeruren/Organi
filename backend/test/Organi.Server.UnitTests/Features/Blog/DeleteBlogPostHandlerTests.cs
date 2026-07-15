using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Blog.Commands.DeleteBlogPost;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Exceptions;
using Xunit;

namespace Organi.Server.UnitTests.Features.Blog;

public sealed class DeleteBlogPostHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly ILogger<DeleteBlogPostHandler> _logger = Substitute.For<ILogger<DeleteBlogPostHandler>>();
    private readonly DeleteBlogPostHandler _handler;

    public DeleteBlogPostHandlerTests()
    {
        _handler = new DeleteBlogPostHandler(_context, _currentUser, _logger);
    }

    private void SetupBlogPosts(params BlogPost[] posts)
    {
        var mockSet = posts.ToList().BuildMockDbSet();
        _context.BlogPosts.Returns(mockSet);
    }

    [Fact]
    public async Task Handle_Owner_RemovesPost()
    {
        var authorId = Guid.NewGuid();
        var post = new BlogPost { AuthorId = authorId, Title = "Title", Slug = "title", Content = "Content" };
        SetupBlogPosts(post);
        _currentUser.UserId.Returns(authorId);
        _currentUser.IsInRole("Admin").Returns(false);

        await _handler.Handle(new DeleteBlogPostCommand(post.Id), CancellationToken.None);

        _context.BlogPosts.Received(1).Remove(post);
    }

    [Fact]
    public async Task Handle_NonOwnerNonAdmin_ThrowsForbiddenException()
    {
        var post = new BlogPost { AuthorId = Guid.NewGuid(), Title = "Title", Slug = "title", Content = "Content" };
        SetupBlogPosts(post);
        _currentUser.UserId.Returns(Guid.NewGuid());
        _currentUser.IsInRole("Admin").Returns(false);

        var act = () => _handler.Handle(new DeleteBlogPostCommand(post.Id), CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task Handle_PostNotFound_ThrowsNotFoundException()
    {
        SetupBlogPosts();
        _currentUser.UserId.Returns(Guid.NewGuid());

        var act = () => _handler.Handle(new DeleteBlogPostCommand(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
