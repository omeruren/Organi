using MediatR;
using Microsoft.EntityFrameworkCore;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Products.DTOs;
using Organi.Server.Application.Features.Products.Mappings;
using Organi.Server.Domain.Enums;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Products.Queries.GetProductById;

public sealed class GetProductByIdHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser) : IRequestHandler<GetProductByIdQuery, ProductResponse>
{
    public async Task<ProductResponse> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Vendor)
            .Include(p => p.ProductImages)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Product", request.Id);

        var isOwnerOrAdmin = currentUser.IsInRole("Admin") || product.VendorId == currentUser.VendorId;
        if (!isOwnerOrAdmin && (product.Status != ProductStatus.Active || product.Vendor.Status != VendorStatus.Approved))
            throw new NotFoundException("Product", request.Id);

        return product.ToResponse();
    }
}
