using FluentValidation.Results;

namespace CleanArchitecture.Blazor.Application.Features.KeyValues.Commands.AddEdit;

public class AddEditKeyValueCommandValidator : AbstractValidator<AddEditKeyValueCommand>
{
    public AddEditKeyValueCommandValidator()
    {
        RuleFor(v => v.Name)
            .NotNull();
        RuleFor(v => v.Text)
            .MaximumLength(256)
            .NotEmpty();
        RuleFor(v => v.Value)
            .MaximumLength(256)
            .NotEmpty();
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue =>
        async (model, propertyName) =>
        {
            ValidationResult? result = await ValidateAsync(ValidationContext<AddEditKeyValueCommand>.CreateWithOptions((AddEditKeyValueCommand)model, x => x.IncludeProperties(propertyName)));
            return result.IsValid ? Array.Empty<string>() : result.Errors.Select(e => e.ErrorMessage);
        };
}