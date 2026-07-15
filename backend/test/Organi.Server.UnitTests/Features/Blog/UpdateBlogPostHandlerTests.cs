using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Blog.Commands.UpdateBlogPost;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Exceptions;
using Xunit;

namespace Organi.Server.UnitTests.Features.Blog;

public sealed class UpdateBlogPostHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly ILogger<UpdateBlogPostHandler> _logger = Substitute.For<ILogger<UpdateBlogPostHandler>>();
    private readonly UpdateBlogPostHandler _handler;

    public UpdateBlogPostHandlerTests()
    {
        _handler = new UpdateBlogPostHandler(_context, _currentUser, _logger);
    }

    private void SetupBlogPosts(params BlogPost[] posts)
    {
        var mockSet = posts.ToList().BuildMockDbSet();
        _context.BlogPosts.Returns(mockSet);
    }

    private static BlogPost BuildPost(Guid authorId, string title = "Original Title", bool isPublished = false)
    {
        var author = new User { Email = "author@organi.test", FirstName = "Ada", LastName = "Lovelace", Id = authorId };
        return new BlogPost { AuthorId = authorId, Author = author, Title = title, Slug = "original-title", Content = "Content", IsPublished = isPublished };
    }

    [Fact]
    public async Task Handle_Owner_TitleChanged_RegeneratesSlug()
    {
        var authorId = Guid.NewGuid();
        var post = BuildPost(authorId);
        SetupBlogPosts(post);
        _currentUser.UserId.Returns(authorId);
        _currentUser.IsInRole("Admin").Returns(false);

        var result = await _handler.Handle(
            new UpdateBlogPostCommand("New Title", "Content", null, null, false) { Id = post.Id }, CancellationToken.None);

        result.Slug.Should().Be("new-title");
    }

    [Fact]
    public async Task Handle_TitleUnchanged_KeepsSlug()
    {
        var authorId = Guid.NewGuid();
        var post = BuildPost(authorId);
        SetupBlogPosts(post);
        _currentUser.UserId.Returns(authorId);
        _currentUser.IsInRole("Admin").Returns(false);

        var result = await _handler.Handle(
            new UpdateBlogPostCommand("Original Title", "New content", null, null, false) { Id = post.Id }, CancellationToken.None);

        result.Slug.Should().Be("original-title");
    }

    [Fact]
    public async Task Handle_TransitionsToPublished_SetsPublishedAt()
    {
        var authorId = Guid.NewGuid();
        var post = BuildPost(authorId, isPublished: false);
        SetupBlogPosts(post);
        _currentUser.UserId.Returns(authorId);
        _currentUser.IsInRole("Admin").Returns(false);

        var result = await _handler.Handle(
            new UpdateBlogPostCommand("Original Title", "Content", null, null, true) { Id = post.Id }, CancellationToken.None);

        result.PublishedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_AlreadyPublished_DoesNotOverwritePublishedAt()
    {
        var authorId = Guid.NewGuid();
        var post = BuildPost(authorId, isPublished: true);
        var originalPublishedAt = DateTime.UtcNow.AddDays(-10);
        post.PublishedAt = originalPublishedAt;
        SetupBlogPosts(post);
        _currentUser.UserId.Returns(authorId);
        _currentUser.IsInRole("Admin").Returns(false);

        var result = await _handler.Handle(
            new UpdateBlogPostCommand("Original Title", "Content", null, null, true) { Id = post.Id }, CancellationToken.None);

        result.PublishedAt.Should().Be(originalPublishedAt);
    }

    [Fact]
    public async Task Handle_NonOwnerNonAdmin_ThrowsForbiddenException()
    {
        var post = BuildPost(Guid.NewGuid());
        SetupBlogPosts(post);
        _currentUser.UserId.Returns(Guid.NewGuid());
        _currentUser.IsInRole("Admin").Returns(false);

        var act = () => _handler.Handle(
            new UpdateBlogPostCommand("New Title", "Content", null, null, false) { Id = post.Id }, CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task Handle_PostNotFound_ThrowsNotFoundException()
    {
        SetupBlogPosts();
        _currentUser.UserId.Returns(Guid.NewGuid());

        var act = () => _handler.Handle(
            new UpdateBlogPostCommand("Title", "Content", null, null, false) { Id = Guid.NewGuid() }, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
