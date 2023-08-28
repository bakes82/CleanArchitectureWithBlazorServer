using FluentValidation.Results;

namespace Application.Example.Features.Documents.Commands.AddEdit;

public class AddEditDocumentCommandValidator : AbstractValidator<AddEditDocumentCommand>
{
    public AddEditDocumentCommandValidator()
    {
        RuleFor(v => v.Title)
            .NotNull()
            .MaximumLength(256)
            .NotEmpty();
        RuleFor(v => v.DocumentType)
            .NotNull();
        RuleFor(v => v.Description)
            .MaximumLength(256);
        RuleFor(v => v.UploadRequest)
            .NotNull()
            .When(x => x.Id <= 0);
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue =>
        async (model, propertyName) =>
        {
            ValidationResult? result = await ValidateAsync(ValidationContext<AddEditDocumentCommand>.CreateWithOptions((AddEditDocumentCommand)model, x => x.IncludeProperties(propertyName)));
            return result.IsValid ? Array.Empty<string>() : result.Errors.Select(e => e.ErrorMessage);
        };
}