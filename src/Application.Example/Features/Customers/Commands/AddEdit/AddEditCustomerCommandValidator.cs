using FluentValidation.Results;

namespace Application.Example.Features.Customers.Commands.AddEdit;

public class AddEditCustomerCommandValidator : AbstractValidator<AddEditCustomerCommand>
{
    public AddEditCustomerCommandValidator()
    {
        // TODO: Implement AddEditCustomerCommandValidator method, for example: 
        // RuleFor(v => v.Name)
        //      .MaximumLength(256)
        //      .NotEmpty();
        throw new NotImplementedException();
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue =>
        async (model, propertyName) =>
        {
            ValidationResult? result = await ValidateAsync(ValidationContext<AddEditCustomerCommand>.CreateWithOptions((AddEditCustomerCommand)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
            {
                return Array.Empty<string>();
            }

            return result.Errors.Select(e => e.ErrorMessage);
        };
}