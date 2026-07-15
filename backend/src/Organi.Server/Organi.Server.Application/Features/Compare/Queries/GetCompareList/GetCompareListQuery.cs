using MediatR;
using Organi.Server.Application.Features.Compare.DTOs;

namespace Organi.Server.Application.Features.Compare.Queries.GetCompareList;

public sealed record GetCompareListQuery : IRequest<IReadOnlyList<CompareItemResponse>>;
