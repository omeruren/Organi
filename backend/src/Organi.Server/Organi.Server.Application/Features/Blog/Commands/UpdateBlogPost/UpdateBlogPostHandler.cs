using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Common.Utilities;
using Organi.Server.Application.Features.Blog.DTOs;
using Organi.Server.Application.Features.Blog.Mappings;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Blog.Commands.UpdateBlogPost;

public sealed class UpdateBlogPostHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser,
    ILogger<UpdateBlogPostHandler> logger) : IRequestHandler<UpdateBlogPostCommand, BlogPostResponse>
{
    public async Task<BlogPostResponse> Handle(UpdateBlogPostCommand request, CancellationToken cancellationToken)
    {
        var post = await context.BlogPosts
            .Include(b => b.Author)
            .Include(b => b.BlogComments)
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("BlogPost", request.Id);

        if (!currentUser.IsInRole("Admin") && post.AuthorId != currentUser.UserId)
            throw new ForbiddenException("You do not have permission to modify this blog post.");

        if (!string.Equals(request.Title, post.Title, StringComparison.Ordinal))
        {
            post.Slug = await SlugGenerator.GenerateUniqueAsync(
                request.Title,
                candidate => context.BlogPosts.AnyAsync(b => b.Slug == candidate && b.Id != post.Id, cancellationToken));
        }

        if (request.IsPublished && !post.IsPublished)
            post.PublishedAt = DateTime.UtcNow;

        post.Title = request.Title;
        post.Content = request.Content;
        post.Excerpt = request.Excerpt;
        post.FeaturedImageUrl = request.FeaturedImageUrl;
        post.IsPublished = request.IsPublished;

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Blog post {BlogPostId} updated by user {UserId}", post.Id, currentUser.UserId);

        return post.ToResponse();
    }
}
