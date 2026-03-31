using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace SharedKernel.UnitOfWork.Behaviors.MediatR;

/// <summary>
/// Represents a behavior in the MediatR pipeline for logging information about
/// incoming requests and their responses. Additionally, it measures the time
/// taken to process a request and logs warnings if the execution time exceeds
/// a predefined threshold.
/// </summary>
/// <typeparam name="TRequest">The type of request handled by the pipeline.</typeparam>
/// <typeparam name="TResponse">The type of response produced by the pipeline.</typeparam>
/// <remarks>
/// This behavior logs the type of the request and response at the start and end of the
/// handling process. It also serializes the response for logging purposes. If the processing
/// time exceeds 3 seconds, a warning is logged to indicate potential performance issues.
/// </remarks>
public class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger
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
        const string prefix = nameof(LoggingBehavior<TRequest, TResponse>);

        logger.LogInformation(
            "[{Prefix}] Handle request={RequestData} and response={ResponseData}",
            prefix,
            typeof(TRequest).Name,
            typeof(TResponse).Name
        );

        var timer = new Stopwatch();
        timer.Start();

        var response = await next();

        timer.Stop();
        var timeTaken = timer.Elapsed;
        if (timeTaken.Seconds > 3)
        {
            logger.LogWarning(
                "[{PerfPossible}] The request {RequestData} took {TimeTaken} seconds",
                prefix,
                typeof(TRequest).Name,
                timeTaken.Seconds
            );
        }

        return response;
    }
}
