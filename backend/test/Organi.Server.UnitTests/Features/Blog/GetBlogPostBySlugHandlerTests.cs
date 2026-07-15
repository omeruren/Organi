using FluentAssertions;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Blog.Queries.GetBlogPostBySlug;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Exceptions;
using Xunit;

namespace Organi.Server.UnitTests.Features.Blog;

public sealed class GetBlogPostBySlugHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly GetBlogPostBySlugHandler _handler;

    public GetBlogPostBySlugHandlerTests()
    {
        _handler = new GetBlogPostBySlugHandler(_context, _currentUser);
    }

    private void SetupBlogPosts(params BlogPost[] posts)
    {
        var mockSet = posts.ToList().BuildMockDbSet();
        _context.BlogPosts.Returns(mockSet);
    }

    [Fact]
    public async Task Handle_PublishedPost_ReturnsBySlug()
    {
        var author = new User { Email = "author@organi.test", FirstName = "Ada", LastName = "Lovelace" };
        var post = new BlogPost { Author = author, AuthorId = author.Id, Title = "Title", Slug = "title", Content = "C", IsPublished = true };
        SetupBlogPosts(post);
        _currentUser.UserId.Returns(Guid.NewGuid());
        _currentUser.IsInRole("Admin").Returns(false);

        var result = await _handler.Handle(new GetBlogPostBySlugQuery("title"), CancellationToken.None);

        result.Slug.Should().Be("title");
    }

    [Fact]
    public async Task Handle_SlugNotFound_ThrowsNotFoundException()
    {
        SetupBlogPosts();
        _currentUser.UserId.Returns(Guid.NewGuid());

        var act = () => _handler.Handle(new GetBlogPostBySlugQuery("missing"), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
