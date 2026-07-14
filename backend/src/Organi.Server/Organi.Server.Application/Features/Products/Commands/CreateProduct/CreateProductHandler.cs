using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Common.Utilities;
using Organi.Server.Application.Features.Products.DTOs;
using Organi.Server.Application.Features.Products.Mappings;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Enums;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Products.Commands.CreateProduct;

public sealed class CreateProductHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser,
    ILogger<CreateProductHandler> logger) : IRequestHandler<CreateProductCommand, ProductResponse>
{
    public async Task<ProductResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var vendorId = currentUser.VendorId
            ?? throw new BusinessRuleException("Only vendors can create products for their own catalog.");

        var vendor = await context.Vendors.FirstOrDefaultAsync(v => v.Id == vendorId, cancellationToken)
            ?? throw new NotFoundException("Vendor", vendorId);

        if (vendor.Status != VendorStatus.Approved)
            throw new BusinessRuleException("Only approved vendors can list products.");

        var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == request.CategoryId, cancellationToken)
            ?? throw new NotFoundException("Category", request.CategoryId);

        if (!category.IsActive)
            throw new BusinessRuleException("Products must belong to an active category.");

        var skuExists = await context.Products.AnyAsync(p => p.SKU == request.SKU, cancellationToken);
        if (skuExists)
            throw new BusinessRuleException($"SKU '{request.SKU}' is already in use.");

        var nameExists = await context.Products
            .AnyAsync(p => p.VendorId == vendorId && p.Name == request.Name, cancellationToken);
        if (nameExists)
            throw new BusinessRuleException($"A product named '{request.Name}' already exists in your catalog.");

        if (request.IsFeatured && (request.Images is null || request.Images.Count == 0))
            throw new BusinessRuleException("Featured products must have at least one image.");

        var slug = await SlugGenerator.GenerateUniqueAsync(
            request.Name,
            candidate => context.Products.AnyAsync(p => p.Slug == candidate, cancellationToken));

        var product = new Product
        {
            Name = request.Name,
            Slug = slug,
            Description = request.Description,
            ShortDescription = request.ShortDescription,
            Price = request.Price,
            SalePrice = request.SalePrice,
            SKU = request.SKU,
            StockQuantity = request.StockQuantity,
            Unit = request.Unit,
            Weight = request.Weight,
            IsOrganic = request.IsOrganic,
            IsFeatured = request.IsFeatured,
            Status = ProductStatus.Draft,
            CategoryId = category.Id,
            Category = category,
            VendorId = vendor.Id,
            Vendor = vendor,
            ProductImages = (request.Images ?? [])
                .Select(i => new ProductImage
                {
                    ImageUrl = i.ImageUrl,
                    AltText = i.AltText,
                    DisplayOrder = i.DisplayOrder,
                    IsPrimary = i.IsPrimary
                })
                .ToList()
        };

        context.Products.Add(product);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Product {ProductId} created by vendor {VendorId}", product.Id, vendorId);

        return product.ToResponse();
    }
}
