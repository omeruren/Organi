using MediatR;
using Microsoft.EntityFrameworkCore;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Compare.DTOs;
using Organi.Server.Application.Features.Compare.Mappings;

namespace Organi.Server.Application.Features.Compare.Queries.GetCompareList;

public sealed class GetCompareListHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser) : IRequestHandler<GetCompareListQuery, IReadOnlyList<CompareItemResponse>>
{
    public async Task<IReadOnlyList<CompareItemResponse>> Handle(GetCompareListQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!.Value;

        // Queries must not modify state (ADR-002) — a user with an empty compare list simply
        // gets an empty list; AddCompareItemHandler is responsible for creating rows.
        var items = await context.CompareItems
            .AsNoTracking()
            .Where(c => c.UserId == userId)
            .Include(c => c.Product)
            .ThenInclude(p => p.ProductImages)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);

        return items.Select(i => i.ToResponse()).ToList();
    }
}
