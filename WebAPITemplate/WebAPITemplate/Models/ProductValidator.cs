using FluentValidation;

namespace WebAPITemplate.Models;

/// <summary>
///     Validates the properties of a product.
/// </summary>
public class ProductValidator : AbstractValidator<Product>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ProductValidator" /> class.
    /// </summary>
    public ProductValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Price must be positive.");

        RuleFor(x => x.DeliveryPrice)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Delivery price must be positive.");
    }
}