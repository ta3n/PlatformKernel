using Microsoft.AspNetCore.Mvc;
using SharedKernel.BulkInsertPipeline.Abstractions;
using SharedKernel.BulkInsertPipeline.Test.Service.Configuration;
using SharedKernel.BulkInsertPipeline.Test.Service.Contracts;
using SharedKernel.BulkInsertPipeline.Test.Service.Data;
using SharedKernel.BulkInsertPipeline.Test.Service.Domain;
using SharedKernel.BulkInsertPipeline.Test.Service.Services;

namespace SharedKernel.BulkInsertPipeline.Test.Service.Api;

public static class BulkInsertPipelineLabEndpoints
{
    public static IEndpointRouteBuilder MapBulkInsertPipelineLabEndpoints(
        this IEndpointRouteBuilder endpoints
    )
    {
        endpoints.MapGet(
            "/",
            (
                BulkInsertPipelineLabDatabase database,
                BulkInsertPipelineLabOptions options
            ) => Results.Ok(
                new
                {
                    service = "SharedKernel.BulkInsertPipeline.Test.Service",
                    plugin = "SharedKernel.BulkInsertPipeline",
                    database = database.DatabaseName,
                    schema = BulkInsertPipelineLabDatabase.Schema,
                    table = BulkInsertPipelineLabDatabase.Table,
                    hostedProvider = options.HostedProvider,
                    fallbackProvider = options.FallbackProvider
                }
            )
        );

        var group = endpoints.MapGroup("/lab/bulk-insert-pipeline")
            .WithTags("BulkInsertPipelineLab");

        group.MapPost("/setup", SetupAsync);
        group.MapGet("/metrics/count", CountAsync);
        group.MapGet("/metrics/recent", RecentAsync);
        group.MapDelete("/metrics", ClearAsync);
        group.MapPost("/metrics/direct", DirectInsertAsync);
        group.MapPost("/metrics/enqueue", EnqueueAsync);

        return endpoints;
    }

    private static async Task<IResult> SetupAsync(
        BulkInsertPipelineLabDatabase database,
        CancellationToken cancellationToken
    )
    {
        await database.EnsureReadyAsync(cancellationToken).ConfigureAwait(false);
        return TypedResults.Ok(
            new
            {
                message = "Bulk insert pipeline lab database is ready.",
                database = database.DatabaseName,
                schema = BulkInsertPipelineLabDatabase.Schema,
                table = BulkInsertPipelineLabDatabase.Table
            }
        );
    }

    private static async Task<IResult> CountAsync(
        BulkInsertPipelineLabDatabase database,
        CancellationToken cancellationToken
    )
    {
        await database.EnsureReadyAsync(cancellationToken).ConfigureAwait(false);
        var totalRows = await database.GetCountAsync(cancellationToken).ConfigureAwait(false);

        return TypedResults.Ok(
            new { totalRows }
        );
    }

    private static async Task<IResult> RecentAsync(
        [FromQuery] int take,
        BulkInsertPipelineLabDatabase database,
        CancellationToken cancellationToken
    )
    {
        if (take <= 0)
        {
            take = 20;
        }

        await database.EnsureReadyAsync(cancellationToken).ConfigureAwait(false);
        var metrics = await database.GetRecentMetricsAsync(take, cancellationToken).ConfigureAwait(false);

        return TypedResults.Ok(
            metrics.Select(
                metric => new
                {
                    metric.Id,
                    metric.DeviceId,
                    metric.OccurredAt,
                    metric.Value,
                    metric.Quality,
                    metric.Source
                }
            )
        );
    }

    private static async Task<IResult> ClearAsync(
        BulkInsertPipelineLabDatabase database,
        CancellationToken cancellationToken
    )
    {
        await database.EnsureReadyAsync(cancellationToken).ConfigureAwait(false);
        await database.ClearAsync(cancellationToken).ConfigureAwait(false);

        return TypedResults.Ok(
            new { message = "Metric events table has been cleared." }
        );
    }

    private static async Task<IResult> DirectInsertAsync(
        InsertMetricsRequest request,
        BulkInsertPipelineLabDatabase database,
        PipelineDirectInsertRunner runner,
        CancellationToken cancellationToken
    )
    {
        var validationProblem = ValidateCount(request.Count, nameof(request.Count));
        if (validationProblem is not null)
        {
            return validationProblem;
        }

        await database.EnsureReadyAsync(cancellationToken).ConfigureAwait(false);

        var batch = MetricReadingFactory.CreateBatch(request.Count, request.DevicePrefix, request.Source);
        var insertedRows = await runner.InsertAsync(request.Provider, batch, cancellationToken)
            .ConfigureAwait(false);
        var totalRows = await database.GetCountAsync(cancellationToken).ConfigureAwait(false);

        return TypedResults.Ok(
            new
            {
                mode = "direct",
                provider = request.Provider,
                requested = request.Count,
                insertedRows,
                totalRows
            }
        );
    }

    private static async Task<IResult> EnqueueAsync(
        EnqueueMetricsRequest request,
        BulkInsertPipelineLabDatabase database,
        BulkInsertPipelineLabOptions options,
        IBulkInsertPipeline<MetricReading> pipeline,
        CancellationToken cancellationToken
    )
    {
        var validationProblem = ValidateCount(request.Count, nameof(request.Count));
        if (validationProblem is not null)
        {
            return validationProblem;
        }

        if (request.WaitForDrainMilliseconds is <= 0)
        {
            return TypedResults.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    [nameof(request.WaitForDrainMilliseconds)] = ["WaitForDrainMilliseconds must be greater than zero when specified."]
                }
            );
        }

        await database.EnsureReadyAsync(cancellationToken).ConfigureAwait(false);

        var batch = MetricReadingFactory.CreateBatch(request.Count, request.DevicePrefix, request.Source);
        foreach (var metric in batch)
        {
            await pipeline.EnqueueAsync(metric, cancellationToken).ConfigureAwait(false);
        }

        var waitForDrainMilliseconds = request.WaitForDrainMilliseconds ?? options.WaitForDrainMilliseconds;
        if (waitForDrainMilliseconds > 0)
        {
            await Task.Delay(waitForDrainMilliseconds, cancellationToken).ConfigureAwait(false);
        }

        var totalRows = await database.GetCountAsync(cancellationToken).ConfigureAwait(false);

        return TypedResults.Ok(
            new
            {
                mode = "pipeline",
                hostedProvider = options.HostedProvider,
                requested = request.Count,
                waitedMilliseconds = waitForDrainMilliseconds,
                totalRows
            }
        );
    }

    private static IResult? ValidateCount(
        int count,
        string fieldName
    )
    {
        if (count <= 0)
        {
            return TypedResults.ValidationProblem(
                new Dictionary<string, string[]> { [fieldName] = ["Count must be greater than zero."] }
            );
        }

        return null;
    }
}
