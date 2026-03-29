using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using SharedKernel.BulkInsertPipeline.Mapping;
using SharedKernel.BulkInsertPipeline.Observability;
using SharedKernel.BulkInsertPipeline.Options;

namespace SharedKernel.BulkInsertPipeline.Providers;

internal sealed class DispatchingBulkInsertService<T> : IBulkInsertService<T>
{
    private readonly BulkInsertEntityDescriptor<T> _descriptor;
    private readonly BulkInsertOptions _options;
    private readonly BulkInsertTelemetry _telemetry;
    private readonly ILogger<DispatchingBulkInsertService<T>> _logger;
    private readonly IReadOnlyDictionary<BulkInsertProviderType, IBulkInsertProvider<T>> _providers;
    private readonly ResiliencePipeline _primaryPipeline;

    public DispatchingBulkInsertService(
        IEnumerable<IBulkInsertProvider<T>> providers,
        BulkInsertEntityDescriptor<T> descriptor,
        BulkInsertOptions options,
        BulkInsertTelemetry telemetry,
        ILogger<DispatchingBulkInsertService<T>> logger
    )
    {
        _descriptor = descriptor;
        _options = options;
        _telemetry = telemetry;
        _logger = logger;
        _providers = providers.ToDictionary(static provider => provider.ProviderType);
        _primaryPipeline = BuildPipeline();
    }

    public async Task InsertAsync(
        ReadOnlyMemory<T> batch,
        CancellationToken ct = default
    )
    {
        if (batch.IsEmpty)
        {
            return;
        }

        if (_descriptor.PartitionRouter is null)
        {
            await InsertToTargetAsync(batch, _descriptor.Table, ct).ConfigureAwait(false);
            return;
        }

        await InsertPartitionedAsync(batch, ct).ConfigureAwait(false);
    }

    private async Task InsertPartitionedAsync(
        ReadOnlyMemory<T> batch,
        CancellationToken ct
    )
    {
        var partitionPlan = BuildPartitionPlan(batch);

        if (partitionPlan.Partitions is null)
        {
            await InsertToTargetAsync(batch, partitionPlan.PrimaryTarget, ct).ConfigureAwait(false);
            return;
        }

        foreach (var partition in partitionPlan.Partitions)
        {
            await InsertToTargetAsync(partition.Value.ToArray(), partition.Key, ct).ConfigureAwait(false);
        }
    }

    private PartitionPlan BuildPartitionPlan(
        ReadOnlyMemory<T> batch
    )
    {
        var router = _descriptor.PartitionRouter!;
        var span = batch.Span;
        var primaryTarget = router.ResolveTarget(span[0], _descriptor.Table);
        var singleTarget = true;

        for (var index = 1; index < span.Length; index++)
        {
            if (router.ResolveTarget(span[index], _descriptor.Table) != primaryTarget)
            {
                singleTarget = false;
                break;
            }
        }

        if (singleTarget)
        {
            return new PartitionPlan(primaryTarget, null);
        }

        Dictionary<BulkInsertTableIdentifier, List<T>> partitions = [];
        foreach (var item in span)
        {
            var target = router.ResolveTarget(item, _descriptor.Table);
            if (!partitions.TryGetValue(target, out var items))
            {
                items = [];
                partitions[target] = items;
            }

            items.Add(item);
        }

        return new PartitionPlan(primaryTarget, partitions);
    }

    private async Task InsertToTargetAsync(
        ReadOnlyMemory<T> batch,
        BulkInsertTableIdentifier target,
        CancellationToken ct
    )
    {
        var primaryProvider = ResolveProvider(_options.PrimaryProvider);
        var executionContext = new BulkInsertExecutionContext<T>(_descriptor, target);
        var stopwatch = Stopwatch.StartNew();
        using var activity = _telemetry.StartInsertActivity(
            _descriptor.EntityName,
            primaryProvider.ProviderType,
            batch.Length,
            target
        );

        try
        {
            await _primaryPipeline.ExecuteAsync(
                    async cancellationToken =>
                    {
                        await primaryProvider.InsertAsync(batch, executionContext, cancellationToken).ConfigureAwait(false);
                    },
                    ct
                )
                .ConfigureAwait(false);

            stopwatch.Stop();
            _telemetry.RecordBatchSucceeded(
                _descriptor.EntityName,
                primaryProvider.ProviderType,
                batch.Length,
                stopwatch.Elapsed
            );
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            stopwatch.Stop();
            _telemetry.RecordBatchFailure(_descriptor.EntityName, primaryProvider.ProviderType, batch.Length);

            var fallbackProvider = ResolveFallbackProvider();
            if (fallbackProvider is not null && fallbackProvider.ProviderType != primaryProvider.ProviderType)
            {
                _logger.LogWarning(
                    exception,
                    "Bulk insert provider {PrimaryProvider} failed for entity {EntityName}. Falling back to provider {FallbackProvider}.",
                    primaryProvider.ProviderType,
                    _descriptor.EntityName,
                    fallbackProvider.ProviderType
                );

                using var fallbackActivity = _telemetry.StartInsertActivity(
                    _descriptor.EntityName,
                    fallbackProvider.ProviderType,
                    batch.Length,
                    target
                );

                var fallbackStopwatch = Stopwatch.StartNew();
                await fallbackProvider.InsertAsync(batch, executionContext, ct).ConfigureAwait(false);
                fallbackStopwatch.Stop();

                _telemetry.RecordBatchSucceeded(
                    _descriptor.EntityName,
                    fallbackProvider.ProviderType,
                    batch.Length,
                    fallbackStopwatch.Elapsed
                );

                return;
            }

            throw;
        }
    }

    private IBulkInsertProvider<T> ResolveProvider(
        BulkInsertProviderType providerType
    )
    {
        if (_providers.TryGetValue(providerType, out var provider))
        {
            return provider;
        }

        throw new InvalidOperationException(
            $"Bulk insert provider '{providerType}' was not registered for entity '{typeof(T).FullName}'."
        );
    }

    private IBulkInsertProvider<T>? ResolveFallbackProvider()
    {
        var fallbackType = _options.Resilience.FallbackProvider;
        if (fallbackType is null)
        {
            return null;
        }

        return _providers.GetValueOrDefault(fallbackType.Value);
    }

    private ResiliencePipeline BuildPipeline()
    {
        var builder = new ResiliencePipelineBuilder();

        if (_options.Resilience.EnableRetry)
        {
            builder.AddRetry(
                new RetryStrategyOptions
                {
                    MaxRetryAttempts = _options.Resilience.MaxRetryAttempts,
                    Delay = _options.Resilience.BaseRetryDelay,
                    UseJitter = true,
                    ShouldHandle = new PredicateBuilder().Handle<Exception>(TransientDatabaseFailureDetector.IsTransient),
                    OnRetry = arguments =>
                    {
                        _logger.LogWarning(
                            arguments.Outcome.Exception,
                            "Retrying bulk insert for entity {EntityName}. Attempt {AttemptNumber}.",
                            _descriptor.EntityName,
                            arguments.AttemptNumber + 1
                        );

                        return default;
                    }
                }
            );
        }

        if (_options.Resilience.EnableCircuitBreaker)
        {
            builder.AddCircuitBreaker(
                new CircuitBreakerStrategyOptions
                {
                    FailureRatio = 0.5d,
                    MinimumThroughput = _options.Resilience.CircuitBreakerFailureThreshold,
                    SamplingDuration = TimeSpan.FromSeconds(30),
                    BreakDuration = _options.Resilience.CircuitBreakerBreakDuration,
                    ShouldHandle = new PredicateBuilder().Handle<Exception>(TransientDatabaseFailureDetector.IsTransient),
                    OnOpened = arguments =>
                    {
                        _logger.LogError(
                            arguments.Outcome.Exception,
                            "Bulk insert circuit opened for entity {EntityName}.",
                            _descriptor.EntityName
                        );

                        return default;
                    },
                    OnClosed = _ =>
                    {
                        _logger.LogInformation(
                            "Bulk insert circuit closed for entity {EntityName}.",
                            _descriptor.EntityName
                        );

                        return default;
                    }
                }
            );
        }

        return builder.Build();
    }

    private sealed record PartitionPlan(
        BulkInsertTableIdentifier PrimaryTarget,
        Dictionary<BulkInsertTableIdentifier, List<T>>? Partitions
    );
}
