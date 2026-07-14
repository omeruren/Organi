using MediatR;
using Microsoft.EntityFrameworkCore;
using Organi.Server.Application.Common.Interfaces;
using Organi.Server.Application.Features.Categories.DTOs;
using Organi.Server.Application.Features.Categories.Mappings;
using Organi.Server.Domain.Exceptions;

namespace Organi.Server.Application.Features.Categories.Queries.GetCategoryById;

public sealed class GetCategoryByIdHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUser) : IRequestHandler<GetCategoryByIdQuery, CategoryResponse>
{
    public async Task<CategoryResponse> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await context.Categories
            .AsNoTracking()
            .Include(c => c.ChildCategories)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Category", request.Id);

        if (!currentUser.IsInRole("Admin") && !category.IsActive)
            throw new NotFoundException("Category", request.Id);

        return category.ToResponse();
    }
}
