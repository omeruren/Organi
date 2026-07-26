using MediatR;
using Microsoft.EntityFrameworkCore;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Blog.DTOs;
using Organi.Server.Application.Features.Blog.Mappings;

namespace Organi.Server.Application.Features.Blog.Queries.GetBlogComments;

public sealed class GetBlogCommentsHandler(
    IApplicationDbContext context) : IRequestHandler<GetBlogCommentsQuery, IReadOnlyList<BlogCommentResponse>>
{
    public async Task<IReadOnlyList<BlogCommentResponse>> Handle(GetBlogCommentsQuery request, CancellationToken cancellationToken)
    {
        var comments = await context.BlogComments
            .AsNoTracking()
            .Include(c => c.User)
            .Where(c => c.BlogPostId == request.BlogPostId && c.IsApproved)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync(cancellationToken);

        return comments.Select(c => c.ToResponse()).ToList();
    }
}
