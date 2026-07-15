using MediatR;
using Organi.Server.Application.Features.Blog.DTOs;

namespace Organi.Server.Application.Features.Blog.Queries.GetBlogPostBySlug;

public sealed record GetBlogPostBySlugQuery(string Slug) : IRequest<BlogPostResponse>;
