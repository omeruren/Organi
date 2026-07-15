using MediatR;
using Organi.Server.Application.Common.Models;
using Organi.Server.Application.Features.Blog.DTOs;

namespace Organi.Server.Application.Features.Blog.Queries.GetBlogPosts;

public sealed record GetBlogPostsQuery(string? Search = null, int Page = 1, int PageSize = 10)
    : IRequest<PagedResponse<BlogPostSummaryResponse>>;
