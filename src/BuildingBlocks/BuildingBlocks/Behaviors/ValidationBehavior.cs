using System.Collections.Immutable;
using BuildingBlocks.CQRS;
using FluentValidation;

namespace BuildingBlocks.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        var context = new ValidationContext<TRequest>(
            instanceToValidate: request
        );

        var validationResults = await Task.WhenAll(
            tasks: validators.Select(selector: v =>
                v.ValidateAsync(
                    context: context,
                    cancellation: cancellationToken
                )
            )
        );

        var failures = validationResults
            .Where(predicate: r => r.Errors.Any())
            .SelectMany(selector: r => r.Errors)
            .ToImmutableArray();

        if (failures.Any())
        {
            throw new ValidationException(errors: failures);
        }

        return await next();
    }
}
