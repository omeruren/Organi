using MediatR;
using Microsoft.EntityFrameworkCore;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Products.DTOs;
using Organi.Server.Application.Features.Products.Mappings;
using Organi.Server.Domain.Enums;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Products.Queries.GetProductBySlug;

public sealed class GetProductBySlugHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser) : IRequestHandler<GetProductBySlugQuery, ProductResponse>
{
    public async Task<ProductResponse> Handle(GetProductBySlugQuery request, CancellationToken cancellationToken)
    {
        var product = await context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Vendor)
            .Include(p => p.ProductImages)
            .FirstOrDefaultAsync(p => p.Slug == request.Slug, cancellationToken)
            ?? throw new NotFoundException("Product", request.Slug);

        var isOwnerOrAdmin = currentUser.IsInRole("Admin") || product.VendorId == currentUser.VendorId;
        if (!isOwnerOrAdmin && (product.Status != ProductStatus.Active || product.Vendor.Status != VendorStatus.Approved))
            throw new NotFoundException("Product", request.Slug);

        return product.ToResponse();
    }
}
