using MediatR;
using Organi.Server.Application.Features.Reviews.DTOs;

namespace Organi.Server.Application.Features.Reviews.Commands.UpdateReview;

public sealed record UpdateReviewCommand(int Rating, string? Title, string? Comment) : IRequest<ReviewResponse>
{
    public Guid Id { get; init; }
}
