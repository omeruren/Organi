using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Blog.Commands.DeleteBlogComment;

public sealed class DeleteBlogCommentHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser,
    ILogger<DeleteBlogCommentHandler> logger) : IRequestHandler<DeleteBlogCommentCommand>
{
    public async Task Handle(DeleteBlogCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = await context.BlogComments.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("BlogComment", request.Id);

        if (!currentUser.IsInRole("Admin") && comment.UserId != currentUser.UserId)
            throw new ForbiddenException("You do not have permission to delete this comment.");

        context.BlogComments.Remove(comment);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Comment {CommentId} deleted by user {UserId}", comment.Id, currentUser.UserId);
    }
}
