using Elsa.Extensions;
using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using SharedKernel.Cache.Inventory;

namespace SharedKernel.WorkflowEngineElsa.Activities;

/// <summary>
/// Releases an existing Redis inventory hold when a booking flow fails, times out, or is cancelled.
/// </summary>
[Activity("SharedKernel.WorkflowEngineElsa", "Booking", "Release a Redis inventory hold for a booking flow.")]
public sealed class ReleaseInventoryHoldActivity(
    IRedisInventoryService redisInventoryService
) : Activity
{
    [Input(Description = "Unique reservation identifier.")]
    public Input<string> ReservationId { get; set; } = default!;

    [Input(Description = "Reason used for compensation and audit.")]
    public Input<string> Reason { get; set; } = new("released");

    [Input(Description = "Reservation metadata retention duration.")]
    public Input<TimeSpan> ReservationDataTtl { get; set; } = new(TimeSpan.FromHours(12));

    [Input(Description = "Optional Redis database index.")]
    public Input<int?> Database { get; set; } = default!;

    [Output(Description = "Redis release result.")]
    public Output<RedisInventoryOperationResult> Result { get; set; } = default!;

    protected override async ValueTask ExecuteAsync(
        ActivityExecutionContext context
    )
    {
        var result = await redisInventoryService.ReleaseAsync(
            new RedisInventoryReleaseRequest
            {
                ReservationId = ReservationId.Get(context),
                Reason = Reason.Get(context),
                ReservationDataTtl = ReservationDataTtl.Get(context),
                Database = Database.Get(context)
            },
            context.CancellationToken
        );

        Result.Set(context, result);
        await context.CompleteActivityAsync();
    }
}
