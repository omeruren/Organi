using MediatR;
using Organi.Server.Application.Features.Categories.DTOs;

namespace Organi.Server.Application.Features.Categories.Queries.GetCategoryById;

public sealed record GetCategoryByIdQuery(Guid Id) : IRequest<CategoryResponse>;
