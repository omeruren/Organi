using MediatR;
using Organi.Server.Application.Features.Compare.DTOs;

namespace Organi.Server.Application.Features.Compare.Commands.AddCompareItem;

public sealed record AddCompareItemCommand(Guid ProductId) : IRequest<CompareItemResponse>;
