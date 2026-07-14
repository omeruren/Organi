using MediatR;
using Microsoft.EntityFrameworkCore;
using Organi.Server.Application.Common.Extensions;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Common.Models;
using Organi.Server.Application.Features.Vendors.DTOs;
using Organi.Server.Domain.Enums;

namespace Organi.Server.Application.Features.Vendors.Queries.GetVendors;

public sealed class GetVendorsHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser) : IRequestHandler<GetVendorsQuery, PagedResponse<VendorResponse>>
{
    public async Task<PagedResponse<VendorResponse>> Handle(GetVendorsQuery request, CancellationToken cancellationToken)
    {
        var query = context.Vendors.AsNoTracking();

        if (!currentUser.IsInRole("Admin"))
            query = query.Where(v => v.Status == VendorStatus.Approved);

        var projected = query
            .OrderBy(v => v.StoreName)
            .Select(v => new VendorResponse(
                v.Id,
                v.StoreName,
                v.Slug,
                v.Description,
                v.LogoUrl,
                v.BannerUrl,
                v.PhoneNumber,
                v.Address,
                v.City,
                v.Rating,
                v.TotalSales,
                v.FollowerCount,
                v.Status.ToString(),
                v.CreatedAt,
                v.UpdatedAt));

        return await projected.ToPagedResponseAsync(request.Page, request.PageSize, cancellationToken);
    }
}
