using ElroukenAljamil.BuildingBlocks.Common.Results;
using FluentValidation;
using MediatR;


namespace ElroukenAljamil.Listings.Application.Behaviors
{
    /// <summary>
    /// Pipeline MediatR qui valide automatiquement les requêtes avant exécution du handler.
    /// </summary>
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
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
                return await next();

            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f is not null)
                .ToList();

            if (failures.Count != 0)
            {
                var errors = failures.Select(f => f.ErrorMessage).ToArray();

                if (typeof(TResponse) == typeof(Result))
                    return (TResponse)(object)Result.Failure(errors.First());

                if (typeof(TResponse).IsGenericType &&
                    typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
                {
                    var failureMethod = typeof(TResponse)
                        .GetMethod(nameof(Result.Failure), new[] { typeof(string) });
                    return (TResponse)failureMethod!.Invoke(null, new object[] { errors.First() })!;
                }

                throw new ValidationException(failures);
            }

            return await next();
        }
    }

}
