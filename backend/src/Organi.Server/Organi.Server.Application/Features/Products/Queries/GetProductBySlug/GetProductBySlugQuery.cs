using MediatR;
using Organi.Server.Application.Features.Products.DTOs;

namespace Organi.Server.Application.Features.Products.Queries.GetProductBySlug;

public sealed record GetProductBySlugQuery(string Slug) : IRequest<ProductResponse>;
