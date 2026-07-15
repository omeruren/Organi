using MediatR;
using Microsoft.EntityFrameworkCore;
using Organi.Server.Application.Common.Extensions;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Common.Models;
using Organi.Server.Application.Features.Blog.DTOs;

namespace Organi.Server.Application.Features.Blog.Queries.GetBlogPosts;

public sealed class GetBlogPostsHandler(
    IApplicationDbContext context) : IRequestHandler<GetBlogPostsQuery, PagedResponse<BlogPostSummaryResponse>>
{
    public async Task<PagedResponse<BlogPostSummaryResponse>> Handle(GetBlogPostsQuery request, CancellationToken cancellationToken)
    {
        var query = context.BlogPosts
            .AsNoTracking()
            .Where(b => b.IsPublished);

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(b => b.Title.Contains(request.Search));

        var projected = query
            .Include(b => b.Author)
            .OrderByDescending(b => b.PublishedAt)
            .Select(b => new BlogPostSummaryResponse(
                b.Id,
                b.Title,
                b.Slug,
                b.Excerpt,
                b.FeaturedImageUrl,
                b.IsPublished,
                b.PublishedAt,
                b.Author.FirstName + " " + b.Author.LastName,
                b.BlogComments.Count,
                b.CreatedAt));

        return await projected.ToPagedResponseAsync(request.Page, request.PageSize, cancellationToken);
    }
}
