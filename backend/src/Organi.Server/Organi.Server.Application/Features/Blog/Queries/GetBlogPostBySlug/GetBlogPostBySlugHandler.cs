using MediatR;
using Microsoft.EntityFrameworkCore;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Blog.DTOs;
using Organi.Server.Application.Features.Blog.Mappings;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Blog.Queries.GetBlogPostBySlug;

public sealed class GetBlogPostBySlugHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser) : IRequestHandler<GetBlogPostBySlugQuery, BlogPostResponse>
{
    public async Task<BlogPostResponse> Handle(GetBlogPostBySlugQuery request, CancellationToken cancellationToken)
    {
        var post = await context.BlogPosts
            .AsNoTracking()
            .Include(b => b.Author)
            .Include(b => b.BlogComments)
            .FirstOrDefaultAsync(b => b.Slug == request.Slug, cancellationToken)
            ?? throw new NotFoundException("BlogPost", request.Slug);

        var isOwnerOrAdmin = currentUser.IsInRole("Admin") || post.AuthorId == currentUser.UserId;
        if (!post.IsPublished && !isOwnerOrAdmin)
            throw new NotFoundException("BlogPost", request.Slug);

        return post.ToResponse();
    }
}
