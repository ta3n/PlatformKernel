using global::Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace SharedKernel.UnitOfWork.Behaviors.Mediator;

/// <summary>
/// Represents a Mediator pipeline behavior that ensures the execution of a request
/// is retried in a fault-tolerant manner using the execution strategy provided by Entity Framework Core.
/// </summary>
/// <typeparam name="TRequest">The type of the request object being processed.</typeparam>
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
    where TRequest : IMessage
{
    /// <summary>
    /// Logger instance used to log informational, warning, and error messages
    /// pertaining to the execution of the <see cref="ExecutionStrategyBehavior{TRequest, TResponse}" />.
    /// </summary>
    private readonly ILogger<ExecutionStrategyBehavior<TRequest, TResponse>> _logger =
        logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Represents the instance of the Entity Framework Core <see cref="DbContext"/> that is used to interact with the database.
    /// This object's primary purpose is to manage database connections and track changes for data persisted to the database.
    /// </summary>
    private readonly DbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    /// <summary>
    /// Handles a request using the specified execution strategy
    /// provided by the associated database context.
    /// </summary>
    /// <param name="request">The request object to be handled.</param>
    /// <param name="next">The delegate to invoke the next behavior or handler in the pipeline.</param>
    /// <param name="cancellationToken">A token that may be used to cancel the request handling operation.</param>
    /// <returns>An awaitable task that produces the response of type <typeparamref name="TResponse"/>.</returns>
    public async ValueTask<TResponse> Handle(
        TRequest message,
        MessageHandlerDelegate<TRequest, TResponse> next,
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

                    result = await next(message, cancellationToken).ConfigureAwait(false);
                },
                cancellationToken
            )
            .ConfigureAwait(false);

        return result;
    }
}
