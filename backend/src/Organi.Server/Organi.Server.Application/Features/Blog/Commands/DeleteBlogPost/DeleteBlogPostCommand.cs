using MediatR;

namespace Organi.Server.Application.Features.Blog.Commands.DeleteBlogPost;

public sealed record DeleteBlogPostCommand(Guid Id) : IRequest;
