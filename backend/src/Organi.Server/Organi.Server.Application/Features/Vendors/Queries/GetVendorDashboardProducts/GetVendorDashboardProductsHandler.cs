using MediatR;
using Microsoft.EntityFrameworkCore;
using Organi.Server.Application.Common.Extensions;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Common.Models;
using Organi.Server.Application.Features.Products.DTOs;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Vendors.Queries.GetVendorDashboardProducts;

public sealed class GetVendorDashboardProductsHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser) : IRequestHandler<GetVendorDashboardProductsQuery, PagedResponse<ProductSummaryResponse>>
{
    public async Task<PagedResponse<ProductSummaryResponse>> Handle(GetVendorDashboardProductsQuery request, CancellationToken cancellationToken)
    {
        var vendorId = currentUser.VendorId
            ?? throw new BusinessRuleException("You do not have a vendor profile.");

        var projected = context.Products
            .AsNoTracking()
            .Where(p => p.VendorId == vendorId)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new ProductSummaryResponse(
                p.Id,
                p.Name,
                p.Slug,
                p.Price,
                p.SalePrice,
                p.Unit,
                p.IsOrganic,
                p.Status.ToString(),
                p.CategoryId,
                p.Category.Name,
                p.VendorId,
                p.Vendor.StoreName,
                p.ProductImages.Where(i => i.IsPrimary).Select(i => i.ImageUrl).FirstOrDefault(),
                p.AverageRating));

        return await projected.ToPagedResponseAsync(request.Page, request.PageSize, cancellationToken);
    }
}
