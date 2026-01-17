using System.Reflection;
using FluentValidation;
using Blueprint.Service.Base.Application.Utils;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SharedKernel.SysException.Exceptions;

namespace Blueprint.Service.Base.Application.Behaviors;

/// <summary>
/// Represents a pipeline behavior in MediatR responsible for validating requests
/// using FluentValidation. If validation fails, it throws an
/// <see cref="AppRequestInvalidException"/> with details of the validation errors.
/// </summary>
/// <typeparam name="TRequest">The type of the request being processed.</typeparam>
/// <typeparam name="TResponse">The type of the response expected from the request.</typeparam>
/// <remarks>
/// This behavior locates validator implementations for the given request type in the
/// specified assembly, executes validation logic, and collects validation failures, if any.
/// </remarks>
/// <example>
/// This behavior is typically registered as part of the request pipeline in MediatR,
/// and is executed for every request that passes through the pipeline.
/// </example>
public class RequestValidationBehavior<TRequest, TResponse>(
    ILogger<RequestValidationBehavior<TRequest, TResponse>> logger,
    [FromKeyedServices("AssemblyForRequestValidationBehavior")]
    Assembly validatorsFromAssembly
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : notnull
{
    /// <summary>
    /// Handles the specified request by validating it using a set of validators retrieved from the provided assembly.
    /// If validation succeeds, the next behavior in the pipeline is called. If validation fails, an exception is thrown.
    /// </summary>
    /// <param name="request">The request object to be validated.</param>
    /// <param name="next">The delegate to invoke the next behavior in the pipeline.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The response of the request if validation passes.</returns>
    /// <exception cref="AppRequestInvalidException">
    /// Thrown when one or more validation failures occur.
    /// </exception>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        var validatorType = typeof(IValidator<>).MakeGenericType(typeof(TRequest));

        var ignoreAttr = request.GetType().GetCustomAttribute<IgnoreValidationAttribute>();
        if (ignoreAttr is not null)
        {
            return await next();
        }

        var validators = validatorsFromAssembly
            .GetTypes()
            .Where(t => validatorType.IsAssignableFrom(t) && t is { IsAbstract: false, IsInterface: false })
            .Select(t => (IValidator<TRequest>)Activator.CreateInstance(t)!)
            .ToList();

        if (validators.Count == 0)
        {
            return await next();
        }

        logger.LogInformation(
            "[{Prefix}] Handle request={RequestData} and response={ResponseData}",
            nameof(RequestValidationBehavior<TRequest, TResponse>),
            typeof(TRequest).Name,
            typeof(TResponse).Name
        );

        var context = new ValidationContext<TRequest>(request);

        var validationFailures = await Task.WhenAll(
            validators.Select(
                x => x.ValidateAsync(context, cancellationToken)
            )
        );

        var validation = validationFailures
            .Where(x => !x.IsValid)
            .SelectMany(x => x.Errors)
            .ToList();

        if (validation.Count > 0)
        {
            throw new AppRequestInvalidException(
                validation.GetErrorCode(),
                validation.GetErrorMessage(),
                validation.GetErrorField()
            );
        }

        return await next();
    }
}
