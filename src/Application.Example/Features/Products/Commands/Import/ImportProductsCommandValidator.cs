namespace Application.Example.Features.Products.Commands.Import;

public class ImportProductsCommandValidator : AbstractValidator<ImportProductsCommand>
{
    public ImportProductsCommandValidator()
    {
        RuleFor(v => v.Data)
            .NotNull()
            .NotEmpty();
    }
}