using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace SharedKernel.UnitOfWork.Behaviors;

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
    /// <summary>
    /// Handles the logging behavior for a request and response lifecycle in the MediatR pipeline.
    /// Logs information about the processing of the request and response, as well as warnings if performance thresholds are exceeded.
    /// </summary>
    /// <param name="request">The incoming request object of type <typeparamref name="TRequest"/>.</param>
    /// <param name="next">Delegate to invoke the next behavior in the pipeline or handle the request.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>The response object of type <typeparamref name="TResponse"/> obtained after processing the request.</returns>
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
        if (timeTaken.Seconds > 3) // if the request is greater than 3 seconds, then log the warnings
        {
            logger.LogWarning(
                "[{PerfPossible}] The request {RequestData} took {TimeTaken} seconds",
                prefix,
                typeof(TRequest).Name,
                timeTaken.Seconds
            );
        }

        // logger.LogInformation(
        //     "[{Prefix}] Handled {RequestData}: {ResponseData}",
        //     prefix,
        //     typeof(TRequest).Name,
        //     JsonConvert.SerializeObject(
        //         response,
        //         new JsonSerializerSettings
        //         {
        //             Error = (
        //                 _,
        //                 args
        //             ) =>
        //             {
        //                 args.ErrorContext.Handled = true;
        //             }
        //         }
        //     )
        // );

        return response;
    }
}
