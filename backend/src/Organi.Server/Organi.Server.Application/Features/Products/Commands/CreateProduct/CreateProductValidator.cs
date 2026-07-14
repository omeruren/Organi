using FluentValidation;

namespace Organi.Server.Application.Features.Products.Commands.CreateProduct;

public sealed class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(200).WithMessage("Product name must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.");

        RuleFor(x => x.ShortDescription)
            .MaximumLength(500).WithMessage("Short description must not exceed 500 characters.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");

        RuleFor(x => x.SalePrice)
            .LessThan(x => x.Price)
            .When(x => x.SalePrice.HasValue)
            .WithMessage("Sale price must be less than the regular price.");

        RuleFor(x => x.SKU)
            .NotEmpty().WithMessage("SKU is required.")
            .MaximumLength(50).WithMessage("SKU must not exceed 50 characters.");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Stock quantity must be zero or greater.");

        RuleFor(x => x.Unit)
            .NotEmpty().WithMessage("Unit is required.")
            .MaximumLength(20).WithMessage("Unit must not exceed 20 characters.");

        RuleFor(x => x.Weight)
            .GreaterThan(0)
            .When(x => x.Weight.HasValue)
            .WithMessage("Weight must be greater than zero when specified.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category is required.");

        RuleForEach(x => x.Images).ChildRules(image =>
        {
            image.RuleFor(i => i.ImageUrl)
                .NotEmpty().WithMessage("Image URL is required.")
                .MaximumLength(500).WithMessage("Image URL must not exceed 500 characters.");

            image.RuleFor(i => i.AltText)
                .MaximumLength(200).WithMessage("Image alt text must not exceed 200 characters.");
        });

        RuleFor(x => x.Images)
            .Must(images => images is null || images.Count(i => i.IsPrimary) <= 1)
            .WithMessage("Only one image can be marked as primary.");
    }
}
