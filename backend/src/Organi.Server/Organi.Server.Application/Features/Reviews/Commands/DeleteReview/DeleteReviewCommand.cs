using MediatR;

namespace Organi.Server.Application.Features.Reviews.Commands.DeleteReview;

public sealed record DeleteReviewCommand(Guid Id) : IRequest;
