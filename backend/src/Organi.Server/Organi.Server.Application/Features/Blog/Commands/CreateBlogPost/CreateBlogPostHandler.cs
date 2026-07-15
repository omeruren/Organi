using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Common.Utilities;
using Organi.Server.Application.Features.Blog.DTOs;
using Organi.Server.Application.Features.Blog.Mappings;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Blog.Commands.CreateBlogPost;

public sealed class CreateBlogPostHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser,
    ILogger<CreateBlogPostHandler> logger) : IRequestHandler<CreateBlogPostCommand, BlogPostResponse>
{
    public async Task<BlogPostResponse> Handle(CreateBlogPostCommand request, CancellationToken cancellationToken)
    {
        var authorId = currentUser.UserId!.Value;

        var author = await context.Users.FirstOrDefaultAsync(u => u.Id == authorId, cancellationToken)
            ?? throw new NotFoundException("User", authorId);

        var slug = await SlugGenerator.GenerateUniqueAsync(
            request.Title,
            candidate => context.BlogPosts.AnyAsync(b => b.Slug == candidate, cancellationToken));

        var post = new BlogPost
        {
            Title = request.Title,
            Slug = slug,
            Content = request.Content,
            Excerpt = request.Excerpt,
            FeaturedImageUrl = request.FeaturedImageUrl,
            IsPublished = request.IsPublished,
            PublishedAt = request.IsPublished ? DateTime.UtcNow : null,
            AuthorId = author.Id,
            Author = author
        };

        context.BlogPosts.Add(post);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Blog post {BlogPostId} created by user {UserId}", post.Id, authorId);

        return post.ToResponse();
    }
}
