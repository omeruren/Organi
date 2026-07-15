using MediatR;
using Organi.Server.Application.Features.Blog.DTOs;

namespace Organi.Server.Application.Features.Blog.Queries.GetBlogPostById;

public sealed record GetBlogPostByIdQuery(Guid Id) : IRequest<BlogPostResponse>;
