using FluentValidation;
using MediatR;

namespace CareConnect.Commands.Behaviors;

/// <summary>
/// Runs every registered FluentValidation validator for a command before it reaches its handler,
/// short-circuiting with a <see cref="ValidationException"/> on failure.
///
/// The constraint is deliberately `notnull` rather than `IRequest&lt;TResponse&gt;`: since MediatR 12
/// a void command's `IRequest` does NOT derive from `IRequest&lt;Unit&gt;`, so the stricter constraint
/// could not be satisfied for void commands and DI silently skipped this behavior for every one of
/// them — their validators never ran.
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next(cancellationToken);
        }

        var context = new ValidationContext<TRequest>(request);

        var failures = (await Task.WhenAll(
                _validators.Select(validator => validator.ValidateAsync(context, cancellationToken))))
            .SelectMany(result => result.Errors)
            .ToList();

        if (failures.Count > 0)
        {
            throw new ValidationException(failures);
        }

        return await next(cancellationToken);
    }
}
