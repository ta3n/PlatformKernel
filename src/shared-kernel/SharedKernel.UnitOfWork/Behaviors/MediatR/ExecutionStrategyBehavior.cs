using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace SharedKernel.UnitOfWork.Behaviors.MediatR;

/// <summary>
/// Represents a MediatR pipeline behavior that ensures the execution of a request
/// is retried in a fault-tolerant manner using the execution strategy provided by Entity Framework Core.
/// </summary>
/// <typeparam name="TRequest">The type of the request object implementing <see cref="IRequest{TResponse}"/>.</typeparam>
/// <typeparam name="TResponse">The type of the response returned after handling the request.</typeparam>
/// <remarks>
/// This behavior makes use of Entity Framework Core's <see cref="IExecutionStrategy"/> for handling transient
/// failures during the execution of a database operation. It wraps the request handling process within the
/// execution strategy's retry logic to ensure resiliency.
/// The behavior logs the beginning and completion of the request handling process for tracking purposes.
/// </remarks>
public class ExecutionStrategyBehavior<TRequest, TResponse>(
    ILogger<ExecutionStrategyBehavior<TRequest, TResponse>> logger,
    DbContext dbContext
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<ExecutionStrategyBehavior<TRequest, TResponse>> _logger =
        logger ?? throw new ArgumentNullException(nameof(logger));

    private readonly DbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        var attemptCount = 0;
        var strategy = _dbContext.Database.CreateExecutionStrategy();

        TResponse result = default!;

        await strategy.ExecuteAsync(
                async _ =>
                {
                    attemptCount++;

                    if (attemptCount > 1)
                    {
                        _logger.LogWarning(
                            "ExecutionStrategy retry attempt {AttemptCount} for {RequestName}",
                            attemptCount,
                            typeof(TRequest).Name
                        );
                    }

                    result = await next().ConfigureAwait(false);
                },
                cancellationToken
            )
            .ConfigureAwait(false);

        return result;
    }
}
