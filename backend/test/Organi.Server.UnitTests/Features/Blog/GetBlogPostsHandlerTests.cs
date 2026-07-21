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
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly GetBlogPostsHandler _handler;

    public GetBlogPostsHandlerTests()
    {
        _handler = new GetBlogPostsHandler(_context, _currentUser);
    }

    private void SetupBlogPosts(params BlogPost[] posts)
    {
        var mockSet = posts.ToList().BuildMockDbSet();
        _context.BlogPosts.Returns(mockSet);
    }

    private static BlogPost BuildPost(string title, bool isPublished, Guid? authorId = null)
    {
        var author = new User
        {
            Id = authorId ?? Guid.NewGuid(),
            Email = $"{title}@organi.test",
            FirstName = "Ada",
            LastName = "Lovelace"
        };

        return new BlogPost
        {
            Title = title,
            Slug = title.ToLowerInvariant().Replace(' ', '-'),
            Content = "C",
            Author = author,
            AuthorId = author.Id,
            IsPublished = isPublished,
            PublishedAt = isPublished ? DateTime.UtcNow : null
        };
    }

    [Fact]
    public async Task Handle_AnonymousCaller_OnlyPublishedPosts_AreReturned()
    {
        SetupBlogPosts(BuildPost("Published", isPublished: true), BuildPost("Draft", isPublished: false));

        var result = await _handler.Handle(new GetBlogPostsQuery(), CancellationToken.None);

        result.Items.Should().ContainSingle();
        result.Items[0].Title.Should().Be("Published");
    }

    [Fact]
    public async Task Handle_Admin_SeesDrafts()
    {
        _currentUser.IsInRole("Admin").Returns(true);
        SetupBlogPosts(BuildPost("Published", isPublished: true), BuildPost("Draft", isPublished: false));

        var result = await _handler.Handle(new GetBlogPostsQuery(), CancellationToken.None);

        result.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_Admin_IsPublishedFalseFilter_ReturnsOnlyDrafts()
    {
        _currentUser.IsInRole("Admin").Returns(true);
        SetupBlogPosts(BuildPost("Published", isPublished: true), BuildPost("Draft", isPublished: false));

        var result = await _handler.Handle(new GetBlogPostsQuery(IsPublished: false), CancellationToken.None);

        result.Items.Should().ContainSingle();
        result.Items[0].Title.Should().Be("Draft");
    }

    [Fact]
    public async Task Handle_Vendor_SeesOwnDraft_ButNotAnotherAuthorsDraft()
    {
        var vendorId = Guid.NewGuid();
        _currentUser.IsInRole("Vendor").Returns(true);
        _currentUser.UserId.Returns(vendorId);

        SetupBlogPosts(
            BuildPost("Own Draft", isPublished: false, authorId: vendorId),
            BuildPost("Other Draft", isPublished: false, authorId: Guid.NewGuid()),
            BuildPost("Published", isPublished: true, authorId: Guid.NewGuid()));

        var result = await _handler.Handle(new GetBlogPostsQuery(), CancellationToken.None);

        result.Items.Select(p => p.Title).Should().BeEquivalentTo("Own Draft", "Published");
    }

    [Fact]
    public async Task Handle_SearchFiltersByTitle()
    {
        SetupBlogPosts(BuildPost("Organic Recipes", isPublished: true), BuildPost("Vendor News", isPublished: true));

        var result = await _handler.Handle(new GetBlogPostsQuery(Search: "Organic"), CancellationToken.None);

        result.Items.Should().ContainSingle();
        result.Items[0].Title.Should().Be("Organic Recipes");
    }
}
