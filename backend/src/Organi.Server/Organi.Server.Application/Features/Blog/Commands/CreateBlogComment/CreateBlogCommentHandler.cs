using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Blog.DTOs;
using Organi.Server.Application.Features.Blog.Mappings;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Blog.Commands.CreateBlogComment;

public sealed class CreateBlogCommentHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser,
    ILogger<CreateBlogCommentHandler> logger) : IRequestHandler<CreateBlogCommentCommand, BlogCommentResponse>
{
    public async Task<BlogCommentResponse> Handle(CreateBlogCommentCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!.Value;

        var post = await context.BlogPosts.FirstOrDefaultAsync(b => b.Id == request.BlogPostId, cancellationToken)
            ?? throw new NotFoundException("BlogPost", request.BlogPostId);

        if (!post.IsPublished)
            throw new BusinessRuleException("You cannot comment on an unpublished blog post.");

        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
            ?? throw new NotFoundException("User", userId);

        var comment = new BlogComment
        {
            Content = request.Content,
            IsApproved = true,
            BlogPostId = post.Id,
            BlogPost = post,
            UserId = user.Id,
            User = user
        };

        context.BlogComments.Add(comment);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Comment {CommentId} added to blog post {BlogPostId} by user {UserId}", comment.Id, post.Id, userId);

        return comment.ToResponse();
    }
}
