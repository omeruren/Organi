using MediatR;

namespace Organi.Server.Application.Features.Compare.Commands.RemoveCompareItem;

public sealed record RemoveCompareItemCommand(Guid ProductId) : IRequest;
