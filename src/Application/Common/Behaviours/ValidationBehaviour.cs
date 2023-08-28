using FluentValidation.Results;

namespace CleanArchitecture.Blazor.Application.Common.Behaviours;

public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        ValidationContext<TRequest> context = new ValidationContext<TRequest>(request);
        List<ValidationFailure> failRules = _validators.Select(validator => validator.Validate(context))
                                                       .SelectMany(result => result.Errors)
                                                       .Where(failRules => failRules != null)
                                                       .ToList();
        if (failRules.Any())
        {
            throw new ValidationException(failRules);
        }

        return await next()
            .ConfigureAwait(false);
    }
}