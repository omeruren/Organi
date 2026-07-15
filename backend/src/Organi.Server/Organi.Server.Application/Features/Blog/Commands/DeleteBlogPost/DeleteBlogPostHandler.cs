using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Blog.Commands.DeleteBlogPost;

public sealed class DeleteBlogPostHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser,
    ILogger<DeleteBlogPostHandler> logger) : IRequestHandler<DeleteBlogPostCommand>
{
    public async Task Handle(DeleteBlogPostCommand request, CancellationToken cancellationToken)
    {
        var post = await context.BlogPosts.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("BlogPost", request.Id);

        if (!currentUser.IsInRole("Admin") && post.AuthorId != currentUser.UserId)
            throw new ForbiddenException("You do not have permission to delete this blog post.");

        context.BlogPosts.Remove(post);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Blog post {BlogPostId} deleted by user {UserId}", post.Id, currentUser.UserId);
    }
}
