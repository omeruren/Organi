using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Blog.Commands.DeleteBlogComment;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Exceptions;
using Xunit;

namespace Organi.Server.UnitTests.Features.Blog;

public sealed class DeleteBlogCommentHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly ILogger<DeleteBlogCommentHandler> _logger = Substitute.For<ILogger<DeleteBlogCommentHandler>>();
    private readonly DeleteBlogCommentHandler _handler;

    public DeleteBlogCommentHandlerTests()
    {
        _handler = new DeleteBlogCommentHandler(_context, _currentUser, _logger);
    }

    private void SetupComments(params BlogComment[] comments)
    {
        var mockSet = comments.ToList().BuildMockDbSet();
        _context.BlogComments.Returns(mockSet);
    }

    [Fact]
    public async Task Handle_Owner_RemovesComment()
    {
        var userId = Guid.NewGuid();
        var comment = new BlogComment { UserId = userId, Content = "Nice!" };
        SetupComments(comment);
        _currentUser.UserId.Returns(userId);
        _currentUser.IsInRole("Admin").Returns(false);

        await _handler.Handle(new DeleteBlogCommentCommand(comment.Id), CancellationToken.None);

        _context.BlogComments.Received(1).Remove(comment);
    }

    [Fact]
    public async Task Handle_NonOwnerNonAdmin_ThrowsForbiddenException()
    {
        var comment = new BlogComment { UserId = Guid.NewGuid(), Content = "Nice!" };
        SetupComments(comment);
        _currentUser.UserId.Returns(Guid.NewGuid());
        _currentUser.IsInRole("Admin").Returns(false);

        var act = () => _handler.Handle(new DeleteBlogCommentCommand(comment.Id), CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task Handle_CommentNotFound_ThrowsNotFoundException()
    {
        SetupComments();
        _currentUser.UserId.Returns(Guid.NewGuid());

        var act = () => _handler.Handle(new DeleteBlogCommentCommand(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
