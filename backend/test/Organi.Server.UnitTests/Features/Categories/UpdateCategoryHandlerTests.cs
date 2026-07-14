using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Categories.Commands.UpdateCategory;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Exceptions;
using Xunit;

namespace Organi.Server.UnitTests.Features.Categories;

public sealed class UpdateCategoryHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ILogger<UpdateCategoryHandler> _logger = Substitute.For<ILogger<UpdateCategoryHandler>>();
    private readonly UpdateCategoryHandler _handler;

    public UpdateCategoryHandlerTests()
    {
        _handler = new UpdateCategoryHandler(_context, _logger);
    }

    private void SetupCategories(params Category[] categories)
    {
        var mockSet = categories.ToList().BuildMockDbSet();
        _context.Categories.Returns(mockSet);
    }

    private static UpdateCategoryCommand ValidCommand(Guid id, Guid? parentCategoryId = null) => new(
        "Vegetables Updated", null, null, 0, true, parentCategoryId)
    { Id = id };

    [Fact]
    public async Task Handle_CategoryNotFound_ThrowsNotFoundException()
    {
        SetupCategories();

        var act = () => _handler.Handle(ValidCommand(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ReparentUnderOwnDescendant_ThrowsBusinessRuleException()
    {
        var parentId = Guid.NewGuid();
        var childId = Guid.NewGuid();

        var parent = new Category { Id = parentId, Name = "Food", Slug = "food", ParentCategoryId = null };
        var child = new Category { Id = childId, Name = "Vegetables", Slug = "vegetables", ParentCategoryId = parentId };

        SetupCategories(parent, child);

        var act = () => _handler.Handle(ValidCommand(parentId, parentCategoryId: childId), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*descendants*");
    }

    [Fact]
    public async Task Handle_DuplicateNameAtSameLevel_ThrowsBusinessRuleException()
    {
        var categoryId = Guid.NewGuid();
        var siblingId = Guid.NewGuid();

        var category = new Category { Id = categoryId, Name = "Fruits", Slug = "fruits", ParentCategoryId = null };
        var sibling = new Category { Id = siblingId, Name = "Vegetables Updated", Slug = "vegetables-updated", ParentCategoryId = null };

        SetupCategories(category, sibling);

        var act = () => _handler.Handle(ValidCommand(categoryId), CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*already exists*");
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsUpdatedCategoryResponse()
    {
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = "Vegetables", Slug = "vegetables", ParentCategoryId = null };

        SetupCategories(category);

        var result = await _handler.Handle(ValidCommand(categoryId), CancellationToken.None);

        result.Name.Should().Be("Vegetables Updated");
        result.Slug.Should().Be("vegetables-updated");
    }
}
