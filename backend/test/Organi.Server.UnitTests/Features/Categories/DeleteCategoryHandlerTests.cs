using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Categories.Commands.DeleteCategory;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Exceptions;
using Xunit;

namespace Organi.Server.UnitTests.Features.Categories;

public sealed class DeleteCategoryHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ILogger<DeleteCategoryHandler> _logger = Substitute.For<ILogger<DeleteCategoryHandler>>();
    private readonly DeleteCategoryHandler _handler;

    public DeleteCategoryHandlerTests()
    {
        _handler = new DeleteCategoryHandler(_context, _logger);
    }

    private void SetupCategories(params Category[] categories)
    {
        var mockSet = categories.ToList().BuildMockDbSet();
        _context.Categories.Returns(mockSet);
    }

    private void SetupProducts(params Product[] products)
    {
        var mockSet = products.ToList().BuildMockDbSet();
        _context.Products.Returns(mockSet);
    }

    [Fact]
    public async Task Handle_CategoryNotFound_ThrowsNotFoundException()
    {
        SetupCategories();

        var act = () => _handler.Handle(new DeleteCategoryCommand(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_CategoryHasProducts_ThrowsBusinessRuleException()
    {
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = "Vegetables", Slug = "vegetables" };
        var product = new Product { Id = Guid.NewGuid(), Name = "Carrot", SKU = "CAR-001", CategoryId = categoryId };

        SetupCategories(category);
        SetupProducts(product);

        var act = () => _handler.Handle(new DeleteCategoryCommand(categoryId), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*products or child categories*");
    }

    [Fact]
    public async Task Handle_CategoryHasChildren_ThrowsBusinessRuleException()
    {
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = "Food", Slug = "food" };
        var child = new Category { Id = Guid.NewGuid(), Name = "Vegetables", Slug = "vegetables", ParentCategoryId = categoryId };

        SetupCategories(category, child);
        SetupProducts();

        var act = () => _handler.Handle(new DeleteCategoryCommand(categoryId), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>();
    }

    [Fact]
    public async Task Handle_NoProductsOrChildren_DeletesCategory()
    {
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = "Vegetables", Slug = "vegetables" };

        SetupCategories(category);
        SetupProducts();

        await _handler.Handle(new DeleteCategoryCommand(categoryId), CancellationToken.None);

        _context.Categories.Received(1).Remove(category);
    }
}
