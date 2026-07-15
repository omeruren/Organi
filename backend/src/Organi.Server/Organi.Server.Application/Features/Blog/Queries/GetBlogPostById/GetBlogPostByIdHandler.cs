using MediatR;
using Microsoft.EntityFrameworkCore;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Blog.DTOs;
using Organi.Server.Application.Features.Blog.Mappings;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Blog.Queries.GetBlogPostById;

public sealed class GetBlogPostByIdHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser) : IRequestHandler<GetBlogPostByIdQuery, BlogPostResponse>
{
    public async Task<BlogPostResponse> Handle(GetBlogPostByIdQuery request, CancellationToken cancellationToken)
    {
        var post = await context.BlogPosts
            .AsNoTracking()
            .Include(b => b.Author)
            .Include(b => b.BlogComments)
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("BlogPost", request.Id);

        var isOwnerOrAdmin = currentUser.IsInRole("Admin") || post.AuthorId == currentUser.UserId;
        if (!post.IsPublished && !isOwnerOrAdmin)
            throw new NotFoundException("BlogPost", request.Id);

        return post.ToResponse();
    }
}
