using MediatR;
using Organi.Server.Application.Features.Categories.DTOs;

namespace Organi.Server.Application.Features.Categories.Queries.GetCategories;

public sealed record GetCategoriesQuery : IRequest<IReadOnlyList<CategoryResponse>>;
