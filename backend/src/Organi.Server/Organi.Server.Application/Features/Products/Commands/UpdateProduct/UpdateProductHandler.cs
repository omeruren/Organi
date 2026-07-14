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

namespace Organi.Server.Application.Features.Products.Commands.UpdateProduct;

public sealed class UpdateProductHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser,
    ILogger<UpdateProductHandler> logger) : IRequestHandler<UpdateProductCommand, ProductResponse>
{
    public async Task<ProductResponse> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await context.Products
            .Include(p => p.Category)
            .Include(p => p.Vendor)
            .Include(p => p.ProductImages)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Product", request.Id);

        if (!currentUser.IsInRole("Admin") && product.VendorId != currentUser.VendorId)
            throw new ForbiddenException("You do not have permission to modify this product.");

        var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == request.CategoryId, cancellationToken)
            ?? throw new NotFoundException("Category", request.CategoryId);

        if (!category.IsActive)
            throw new BusinessRuleException("Products must belong to an active category.");

        var skuExists = await context.Products
            .AnyAsync(p => p.SKU == request.SKU && p.Id != product.Id, cancellationToken);
        if (skuExists)
            throw new BusinessRuleException($"SKU '{request.SKU}' is already in use.");

        var nameExists = await context.Products
            .AnyAsync(p => p.VendorId == product.VendorId && p.Name == request.Name && p.Id != product.Id, cancellationToken);
        if (nameExists)
            throw new BusinessRuleException($"A product named '{request.Name}' already exists in your catalog.");

        var finalImageCount = request.Images?.Count ?? product.ProductImages.Count;
        if (request.IsFeatured && finalImageCount == 0)
            throw new BusinessRuleException("Featured products must have at least one image.");

        if (!string.Equals(request.Name, product.Name, StringComparison.Ordinal))
        {
            product.Slug = await SlugGenerator.GenerateUniqueAsync(
                request.Name,
                candidate => context.Products.AnyAsync(p => p.Slug == candidate && p.Id != product.Id, cancellationToken));
        }

        product.Name = request.Name;
        product.Description = request.Description;
        product.ShortDescription = request.ShortDescription;
        product.Price = request.Price;
        product.SalePrice = request.SalePrice;
        product.SKU = request.SKU;
        product.StockQuantity = request.StockQuantity;
        product.Unit = request.Unit;
        product.Weight = request.Weight;
        product.IsOrganic = request.IsOrganic;
        product.IsFeatured = request.IsFeatured;
        product.Status = Enum.Parse<ProductStatus>(request.Status, ignoreCase: true);
        product.CategoryId = category.Id;
        product.Category = category;

        if (request.Images is not null)
        {
            context.ProductImages.RemoveRange(product.ProductImages);

            var newImages = request.Images
                .Select(i => new ProductImage
                {
                    ImageUrl = i.ImageUrl,
                    AltText = i.AltText,
                    DisplayOrder = i.DisplayOrder,
                    IsPrimary = i.IsPrimary,
                    ProductId = product.Id
                })
                .ToList();

            // Explicitly Add: these have client-generated non-default Guid keys, so EF's
            // change tracker would otherwise treat them as existing rows to UPDATE rather than INSERT.
            context.ProductImages.AddRange(newImages);
            product.ProductImages = newImages;
        }

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Product {ProductId} updated by user {UserId}", product.Id, currentUser.UserId);

        return product.ToResponse();
    }
}
