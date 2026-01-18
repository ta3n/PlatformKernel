using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Liberty.UnitOfWork.Behaviors;

public class ExecutionStrategyBehavior<TRequest, TResponse>(
    ILogger<ExecutionStrategyBehavior<TRequest, TResponse>> logger,
    DbContext dbContext
)
    : IPipelineBehavior<TRequest, TResponse>
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
        var response = default(TResponse)!;
        var strategy = _dbContext.Database.CreateExecutionStrategy();

        try
        {
            await strategy.ExecuteAsync(
                    async () =>
                    {
                        _logger.LogInformation(
                            "ExecutionStrategyBehavior - Starting request handling for {RequestName}",
                            typeof(TRequest).Name
                        );

                        response = await next();

                        _logger.LogInformation(
                            "ExecutionStrategyBehavior - Completed request handling for {RequestName}",
                            typeof(TRequest).Name
                        );
                    }
                )
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An error occurred while handling the request {RequestName}",
                typeof(TRequest).Name
            );
            throw;
        }

        return response;
    }
}
