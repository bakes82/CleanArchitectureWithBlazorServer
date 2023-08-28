using FluentValidation.Results;

namespace Application.Example.Features.Products.Commands.AddEdit;

public class AddEditProductCommandValidator : AbstractValidator<AddEditProductCommand>
{
    public AddEditProductCommandValidator()
    {
        RuleFor(v => v.Name)
            .MaximumLength(256)
            .NotEmpty();
        RuleFor(v => v.Unit)
            .MaximumLength(2)
            .NotEmpty();
        RuleFor(v => v.Brand)
            .MaximumLength(30)
            .NotEmpty();
        RuleFor(v => v.Price)
            .GreaterThanOrEqualTo(0);
        RuleFor(v => v.Description)
            .MaximumLength(1024);
        RuleFor(v => v.Pictures)
            .NotEmpty()
            .WithMessage("Please upload product pictures.");
        RuleFor(v => v.UploadPictures)
            .NotEmpty()
            .When(x => x.Pictures == null || !x.Pictures.Any())
            .WithMessage("Please upload product pictures.");
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue =>
        async (model, propertyName) =>
        {
            ValidationResult? result = await ValidateAsync(ValidationContext<AddEditProductCommand>.CreateWithOptions((AddEditProductCommand)model, x => x.IncludeProperties(propertyName)));
            return result.IsValid ? Array.Empty<string>() : result.Errors.Select(e => e.ErrorMessage);
        };
}