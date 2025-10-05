using Core.Helpers;
using FluentValidation;
using MediatR;

namespace Core.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, 
            RequestHandlerDelegate<TResponse> next, 
            CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);
                var validationResults = await Task.WhenAll(
                    _validators.Select(v => v.ValidateAsync(context, cancellationToken))
                );

                var failures = validationResults
                    .SelectMany(r => r.Errors)
                    .Where(f => f != null)
                    .ToList();

                if (failures.Count > 0)
                {
                    var errorMessage = string.Join("; ", failures.Select(f => f.ErrorMessage));

                    // figure out TResponse type (e.g. ApiResponse<AuthResponseDto>)
                    var responseType = typeof(TResponse);

                    if (responseType.IsGenericType &&
                        responseType.GetGenericTypeDefinition() == typeof(ApiResponse<>))
                    {
                        var innerType = responseType.GetGenericArguments()[0];

                        // construct ApiResponse<innerType>.Fail(errorMessage)
                        var method = typeof(ApiResponse<>)
                            .MakeGenericType(innerType)
                            .GetMethod(nameof(ApiResponse<object>.Fail));

                        var failResponse = method!.Invoke(null, [errorMessage]);

                        return (TResponse)failResponse!;
                    }

                    throw new ValidationException(failures);
                }
            }

            return await next(cancellationToken);
        }
    }
}
