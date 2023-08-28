namespace CleanArchitecture.Blazor.Application.Features.KeyValues.Commands.Import;

public class ImportKeyValuesCommandValidator : AbstractValidator<ImportKeyValuesCommand>
{
    public ImportKeyValuesCommandValidator()
    {
        RuleFor(x => x.Data)
            .NotNull()
            .NotEmpty();
    }
}