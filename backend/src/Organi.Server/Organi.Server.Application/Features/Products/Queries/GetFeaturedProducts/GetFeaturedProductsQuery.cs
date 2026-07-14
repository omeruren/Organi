using MediatR;
using Organi.Server.Application.Features.Products.DTOs;

namespace Organi.Server.Application.Features.Products.Queries.GetFeaturedProducts;

public sealed record GetFeaturedProductsQuery(int Take = 8) : IRequest<IReadOnlyList<ProductSummaryResponse>>;
