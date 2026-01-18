using FluentValidation;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Exceptions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.Application.Behaviors;

public class RequestValidationBehavior<TRequest, TResponse>(
    ILogger<RequestValidationBehavior<TRequest, TResponse>> logger,
    IServiceProvider serviceProvider
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation(
            "[{Prefix}] Handle request={RequestData} and response={ResponseData}",
            nameof(RequestValidationBehavior<TRequest, TResponse>),
            typeof(TRequest).Name,
            typeof(TResponse).Name
        );

        var validator = serviceProvider.GetService<IValidator<TRequest>>();
        if (validator is not null)
        {
            var validationResult = await validator.ValidateAsync(
                request,
                cancellationToken
            );
            if (!validationResult.IsValid)
            {
                throw new RequestInvalidException(
                    ErrorCode.E0000x,
                    validationResult.ToString()
                );
            }
        }

        var response = await next();

        return response;
    }
}
