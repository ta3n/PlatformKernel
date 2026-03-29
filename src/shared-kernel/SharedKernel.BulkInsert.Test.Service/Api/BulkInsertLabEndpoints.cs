using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.BulkInsert.Abstractions;
using SharedKernel.BulkInsert.Models;
using SharedKernel.BulkInsert.Test.Service.Contracts;
using SharedKernel.BulkInsert.Test.Service.Data;
using SharedKernel.BulkInsert.Test.Service.Domain;

namespace SharedKernel.BulkInsert.Test.Service.Api;

public static class BulkInsertLabEndpoints
{
    public static IEndpointRouteBuilder MapBulkInsertLabEndpoints(
        this IEndpointRouteBuilder endpoints
    )
    {
        endpoints.MapGet(
            "/",
            (
                BulkInsertLabDatabase database
            ) => Results.Ok(
                new
                {
                    service = "SharedKernel.BulkInsert.Test.Service",
                    plugin = "SharedKernel.BulkInsert",
                    database = database.DatabaseName,
                    schema = BulkInsertLabDatabase.Schema,
                    table = BulkInsertLabDatabase.Table
                }
            )
        );

        var group = endpoints.MapGroup("/lab/bulk-insert")
            .WithTags("BulkInsertLab");

        group.MapPost("/setup", SetupAsync);
        group.MapGet("/orders/count", CountAsync);
        group.MapGet("/orders/recent", RecentAsync);
        group.MapDelete("/orders", ClearAsync);
        group.MapPost("/orders/direct", BulkInsertDirectAsync);
        group.MapPost("/orders/fluent", BulkInsertFluentAsync);

        return endpoints;
    }

    private static async Task<IResult> SetupAsync(
        BulkInsertLabDatabase database,
        CancellationToken cancellationToken
    )
    {
        await database.EnsureReadyAsync(cancellationToken).ConfigureAwait(false);
        return TypedResults.Ok(
            new
            {
                message = "Bulk insert lab database is ready.",
                database = database.DatabaseName,
                schema = BulkInsertLabDatabase.Schema,
                table = BulkInsertLabDatabase.Table
            }
        );
    }

    private static async Task<IResult> CountAsync(
        BulkInsertLabDatabase database,
        CancellationToken cancellationToken
    )
    {
        await database.EnsureReadyAsync(cancellationToken).ConfigureAwait(false);
        var totalRows = await database.GetCountAsync(cancellationToken).ConfigureAwait(false);

        return TypedResults.Ok(
            new
            {
                totalRows
            }
        );
    }

    private static async Task<IResult> RecentAsync(
        [FromQuery] int take,
        BulkInsertLabDatabase database,
        CancellationToken cancellationToken
    )
    {
        if (take <= 0)
        {
            take = 20;
        }

        await database.EnsureReadyAsync(cancellationToken).ConfigureAwait(false);
        var orders = await database.GetRecentOrdersAsync(take, cancellationToken).ConfigureAwait(false);

        return TypedResults.Ok(
            orders.Select(
                order => new
                {
                    order.Id,
                    order.ExternalId,
                    order.Quantity,
                    order.CreatedAtUtc,
                    order.Source
                }
            )
        );
    }

    private static async Task<IResult> ClearAsync(
        BulkInsertLabDatabase database,
        CancellationToken cancellationToken
    )
    {
        await database.EnsureReadyAsync(cancellationToken).ConfigureAwait(false);
        await database.ClearAsync(cancellationToken).ConfigureAwait(false);

        return TypedResults.Ok(
            new
            {
                message = "Orders table has been cleared."
            }
        );
    }

    private static async Task<IResult> BulkInsertDirectAsync(
        BulkInsertOrdersRequest request,
        BulkInsertLabDatabase database,
        IDbContextFactory<BulkInsertLabDbContext> dbContextFactory,
        IPostgreSqlBulkInsertService bulkInsertService,
        CancellationToken cancellationToken
    )
    {
        var validationProblem = ValidateRequest(request);
        if (validationProblem is not null)
        {
            return validationProblem;
        }

        await database.EnsureReadyAsync(cancellationToken).ConfigureAwait(false);

        var orders = CreateOrders(request);
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken)
            .ConfigureAwait(false);

        var insertedRows = await bulkInsertService.BulkInsertAsync(
                dbContext,
                orders,
                request.Provider,
                CreateOptions(request),
                cancellationToken
            )
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

    private static async Task<IResult> BulkInsertFluentAsync(
        BulkInsertOrdersRequest request,
        BulkInsertLabDatabase database,
        IFluentBulkInsertService<LabOrder> fluentBulkInsertService,
        CancellationToken cancellationToken
    )
    {
        var validationProblem = ValidateRequest(request);
        if (validationProblem is not null)
        {
            return validationProblem;
        }

        await database.EnsureReadyAsync(cancellationToken).ConfigureAwait(false);

        var orders = CreateOrders(request);
        var insertedRows = await fluentBulkInsertService.BulkInsertAsync(
                orders,
                request.Provider,
                CreateOptions(request),
                cancellationToken
            )
            .ConfigureAwait(false);

        var totalRows = await database.GetCountAsync(cancellationToken).ConfigureAwait(false);

        return TypedResults.Ok(
            new
            {
                mode = "fluent",
                provider = request.Provider,
                requested = request.Count,
                insertedRows,
                totalRows
            }
        );
    }

    private static IResult? ValidateRequest(
        BulkInsertOrdersRequest request
    )
    {
        if (request.Count <= 0)
        {
            return TypedResults.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    [nameof(request.Count)] = ["Count must be greater than zero."]
                }
            );
        }

        if (request.BatchSize is <= 0)
        {
            return TypedResults.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    [nameof(request.BatchSize)] = ["BatchSize must be greater than zero when specified."]
                }
            );
        }

        if (request.TimeoutSeconds is <= 0)
        {
            return TypedResults.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    [nameof(request.TimeoutSeconds)] = ["TimeoutSeconds must be greater than zero when specified."]
                }
            );
        }

        return null;
    }

    private static PostgreSqlBulkInsertOptions CreateOptions(
        BulkInsertOrdersRequest request
    ) => new()
    {
        BatchSize = request.BatchSize,
        TimeoutSeconds = request.TimeoutSeconds
    };

    private static LabOrder[] CreateOrders(
        BulkInsertOrdersRequest request
    )
    {
        var count = request.Count;
        var prefix = string.IsNullOrWhiteSpace(request.Prefix) ? "order" : request.Prefix.Trim();
        var source = string.IsNullOrWhiteSpace(request.Source) ? "mini-api" : request.Source.Trim();
        var batchToken = Guid.NewGuid().ToString("N");
        var createdAt = DateTime.UtcNow;
        var orders = new LabOrder[count];

        for (var index = 0; index < count; index++)
        {
            orders[index] = new LabOrder
            {
                ExternalId = string.Concat(
                    prefix,
                    "-",
                    batchToken,
                    "-",
                    index.ToString("D4", CultureInfo.InvariantCulture)
                ),
                Quantity = (index + 1) * 10,
                CreatedAtUtc = createdAt.AddSeconds(index),
                Source = source
            };
        }

        return orders;
    }
}
