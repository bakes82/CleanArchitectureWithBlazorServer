namespace Application.Example.Features.Products.Commands.Delete;

public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotNull()
            .ForEach(v => v.NotEqual(0));
    }
}