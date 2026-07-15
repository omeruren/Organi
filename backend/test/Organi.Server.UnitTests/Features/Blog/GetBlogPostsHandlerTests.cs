using FluentAssertions;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Blog.Queries.GetBlogPosts;
using Organi.Server.Domain.Entities;
using Xunit;

namespace Organi.Server.UnitTests.Features.Blog;

public sealed class GetBlogPostsHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly GetBlogPostsHandler _handler;

    public GetBlogPostsHandlerTests()
    {
        _handler = new GetBlogPostsHandler(_context);
    }

    private void SetupBlogPosts(params BlogPost[] posts)
    {
        var mockSet = posts.ToList().BuildMockDbSet();
        _context.BlogPosts.Returns(mockSet);
    }

    [Fact]
    public async Task Handle_OnlyPublishedPosts_AreReturned()
    {
        var author = new User { Email = "author@organi.test", FirstName = "Ada", LastName = "Lovelace" };
        var published = new BlogPost { Title = "Published", Slug = "published", Content = "C", Author = author, IsPublished = true, PublishedAt = DateTime.UtcNow };
        var draft = new BlogPost { Title = "Draft", Slug = "draft", Content = "C", Author = author, IsPublished = false };
        SetupBlogPosts(published, draft);

        var result = await _handler.Handle(new GetBlogPostsQuery(), CancellationToken.None);

        result.Items.Should().ContainSingle();
        result.Items[0].Title.Should().Be("Published");
    }

    [Fact]
    public async Task Handle_SearchFiltersByTitle()
    {
        var author = new User { Email = "author@organi.test", FirstName = "Ada", LastName = "Lovelace" };
        var matching = new BlogPost { Title = "Organic Recipes", Slug = "organic-recipes", Content = "C", Author = author, IsPublished = true, PublishedAt = DateTime.UtcNow };
        var nonMatching = new BlogPost { Title = "Vendor News", Slug = "vendor-news", Content = "C", Author = author, IsPublished = true, PublishedAt = DateTime.UtcNow };
        SetupBlogPosts(matching, nonMatching);

        var result = await _handler.Handle(new GetBlogPostsQuery(Search: "Organic"), CancellationToken.None);

        result.Items.Should().ContainSingle();
        result.Items[0].Title.Should().Be("Organic Recipes");
    }
}
