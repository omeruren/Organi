using MediatR;
using Organi.Server.Application.Features.Reviews.DTOs;

namespace Organi.Server.Application.Features.Reviews.Commands.CreateReview;

public sealed record CreateReviewCommand(int Rating, string? Title, string? Comment) : IRequest<ReviewResponse>
{
    public Guid ProductId { get; init; }
}
