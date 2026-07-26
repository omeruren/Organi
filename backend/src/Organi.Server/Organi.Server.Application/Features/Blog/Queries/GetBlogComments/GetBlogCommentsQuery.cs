using MediatR;
using Organi.Server.Application.Features.Blog.DTOs;

namespace Organi.Server.Application.Features.Blog.Queries.GetBlogComments;

public sealed record GetBlogCommentsQuery(Guid BlogPostId) : IRequest<IReadOnlyList<BlogCommentResponse>>;
