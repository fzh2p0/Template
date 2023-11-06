using FluentValidation;

namespace WebAPITemplate.Models;

/// <summary>
///     Validates the properties of a product option.
/// </summary>
public class ProductOptionValidator : AbstractValidator<ProductOption>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ProductOptionValidator" /> class.
    /// </summary>
    public ProductOptionValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.");
    }
}