using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Categories.Commands.CreateCategory;
using Organi.Server.Domain.Entities;
using Organi.Server.Domain.Exceptions;
using Xunit;

namespace Organi.Server.UnitTests.Features.Categories;

public sealed class CreateCategoryHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ILogger<CreateCategoryHandler> _logger = Substitute.For<ILogger<CreateCategoryHandler>>();
    private readonly CreateCategoryHandler _handler;

    public CreateCategoryHandlerTests()
    {
        _handler = new CreateCategoryHandler(_context, _logger);
    }

    private void SetupCategories(params Category[] categories)
    {
        var mockSet = categories.ToList().BuildMockDbSet();
        _context.Categories.Returns(mockSet);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsCategoryResponse()
    {
        SetupCategories();

        var command = new CreateCategoryCommand("Vegetables", "Fresh vegetables.", null, 0, null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Name.Should().Be("Vegetables");
        result.Slug.Should().Be("vegetables");
    }

    [Fact]
    public async Task Handle_DuplicateNameAtSameLevel_ThrowsBusinessRuleException()
    {
        var existing = new Category { Id = Guid.NewGuid(), Name = "Vegetables", Slug = "vegetables", ParentCategoryId = null };
        SetupCategories(existing);

        var command = new CreateCategoryCommand("Vegetables", null, null, 0, null);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*already exists*");
    }

    [Fact]
    public async Task Handle_ParentNotFound_ThrowsNotFoundException()
    {
        SetupCategories();

        var command = new CreateCategoryCommand("Leafy Greens", null, null, 0, Guid.NewGuid());

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ExceedsMaxNestingDepth_ThrowsBusinessRuleException()
    {
        var rootId = Guid.NewGuid();
        var childId = Guid.NewGuid();
        var grandchildId = Guid.NewGuid();

        var root = new Category { Id = rootId, Name = "Food", Slug = "food", ParentCategoryId = null };
        var child = new Category { Id = childId, Name = "Vegetables", Slug = "vegetables", ParentCategoryId = rootId };
        var grandchild = new Category { Id = grandchildId, Name = "Leafy Greens", Slug = "leafy-greens", ParentCategoryId = childId };

        SetupCategories(root, child, grandchild);

        var command = new CreateCategoryCommand("Spinach", null, null, 0, grandchildId);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*nesting depth*");
    }
}
