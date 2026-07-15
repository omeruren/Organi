using MediatR;

namespace Organi.Server.Application.Features.Blog.Commands.DeleteBlogComment;

public sealed record DeleteBlogCommentCommand(Guid Id) : IRequest;
