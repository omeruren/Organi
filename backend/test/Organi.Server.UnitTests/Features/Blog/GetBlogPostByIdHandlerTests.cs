using FluentAssertions;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Blog.Queries.GetBlogPostById;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Exceptions;
using Xunit;

namespace Organi.Server.UnitTests.Features.Blog;

public sealed class GetBlogPostByIdHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly GetBlogPostByIdHandler _handler;

    public GetBlogPostByIdHandlerTests()
    {
        _handler = new GetBlogPostByIdHandler(_context, _currentUser);
    }

    private void SetupBlogPosts(params BlogPost[] posts)
    {
        var mockSet = posts.ToList().BuildMockDbSet();
        _context.BlogPosts.Returns(mockSet);
    }

    private static BlogPost BuildPost(Guid authorId, bool isPublished)
    {
        var author = new User { Email = "author@organi.test", FirstName = "Ada", LastName = "Lovelace", Id = authorId };
        return new BlogPost { AuthorId = authorId, Author = author, Title = "Title", Slug = "title", Content = "C", IsPublished = isPublished };
    }

    [Fact]
    public async Task Handle_PublishedPost_AnyoneCanView()
    {
        var post = BuildPost(Guid.NewGuid(), isPublished: true);
        SetupBlogPosts(post);
        _currentUser.UserId.Returns(Guid.NewGuid());
        _currentUser.IsInRole("Admin").Returns(false);

        var result = await _handler.Handle(new GetBlogPostByIdQuery(post.Id), CancellationToken.None);

        result.Id.Should().Be(post.Id);
    }

    [Fact]
    public async Task Handle_UnpublishedPost_NonOwnerNonAdmin_ThrowsNotFoundException()
    {
        var post = BuildPost(Guid.NewGuid(), isPublished: false);
        SetupBlogPosts(post);
        _currentUser.UserId.Returns(Guid.NewGuid());
        _currentUser.IsInRole("Admin").Returns(false);

        var act = () => _handler.Handle(new GetBlogPostByIdQuery(post.Id), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_UnpublishedPost_Owner_CanView()
    {
        var authorId = Guid.NewGuid();
        var post = BuildPost(authorId, isPublished: false);
        SetupBlogPosts(post);
        _currentUser.UserId.Returns(authorId);
        _currentUser.IsInRole("Admin").Returns(false);

        var result = await _handler.Handle(new GetBlogPostByIdQuery(post.Id), CancellationToken.None);

        result.Id.Should().Be(post.Id);
    }
}
