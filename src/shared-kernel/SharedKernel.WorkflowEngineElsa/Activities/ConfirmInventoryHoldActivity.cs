using Elsa.Extensions;
using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using SharedKernel.Cache.Inventory;

namespace SharedKernel.WorkflowEngineElsa.Activities;

/// <summary>
/// Confirms an existing Redis inventory hold after the durable booking transaction and outbox commit succeed.
/// </summary>
[Activity("SharedKernel.WorkflowEngineElsa", "Booking", "Confirm a Redis inventory hold for a booking flow.")]
public sealed class ConfirmInventoryHoldActivity(
    IRedisInventoryService redisInventoryService
) : Activity
{
    [Input(Description = "Unique reservation identifier.")]
    public Input<string> ReservationId { get; set; } = default!;

    [Input(Description = "Reservation metadata retention duration.")]
    public Input<TimeSpan> ReservationDataTtl { get; set; } = new(TimeSpan.FromHours(12));

    [Input(Description = "Optional Redis database index.")]
    public Input<int?> Database { get; set; } = default!;

    [Output(Description = "Redis confirmation result.")]
    public Output<RedisInventoryOperationResult> Result { get; set; } = default!;

    protected override async ValueTask ExecuteAsync(
        ActivityExecutionContext context
    )
    {
        var result = await redisInventoryService.ConfirmAsync(
            new RedisInventoryConfirmRequest
            {
                ReservationId = ReservationId.Get(context),
                ReservationDataTtl = ReservationDataTtl.Get(context),
                Database = Database.Get(context)
            },
            context.CancellationToken
        );

        Result.Set(context, result);
        await context.CompleteActivityAsync();
    }
}
