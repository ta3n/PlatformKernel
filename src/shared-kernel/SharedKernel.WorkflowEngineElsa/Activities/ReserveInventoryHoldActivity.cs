using Elsa.Extensions;
using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using SharedKernel.Cache.Inventory;

namespace SharedKernel.WorkflowEngineElsa.Activities;

/// <summary>
/// Reserves a short-lived Redis inventory hold to protect booking flows from overbooking before the booking row is committed.
/// </summary>
[Activity("SharedKernel.WorkflowEngineElsa", "Booking", "Reserve a Redis inventory hold for a booking flow.")]
public sealed class ReserveInventoryHoldActivity(
    IRedisInventoryService redisInventoryService
) : Activity
{
    [Input(Description = "Unique reservation identifier.")]
    public Input<string> ReservationId { get; set; } = default!;

    [Input(Description = "Inventory identifier.")]
    public Input<string> InventoryId { get; set; } = default!;

    [Input(Description = "Dates to reserve.")]
    public Input<ICollection<DateOnly>> Dates { get; set; } = default!;

    [Input(Description = "Quantity to reserve.")]
    public Input<long> Quantity { get; set; } = new(1);

    [Input(Description = "Redis hold duration.")]
    public Input<TimeSpan> HoldTtl { get; set; } = new(TimeSpan.FromMinutes(10));

    [Input(Description = "Reservation metadata retention duration.")]
    public Input<TimeSpan> ReservationDataTtl { get; set; } = new(TimeSpan.FromHours(12));

    [Input(Description = "Optional Redis database index.")]
    public Input<int?> Database { get; set; } = default!;

    [Output(Description = "Redis reservation result.")]
    public Output<RedisInventoryOperationResult> Result { get; set; } = default!;

    protected override async ValueTask ExecuteAsync(
        ActivityExecutionContext context
    )
    {
        var request = new RedisInventoryReservationRequest
        {
            ReservationId = ReservationId.Get(context),
            InventoryId = InventoryId.Get(context),
            Dates = Dates.Get(context).ToArray(),
            Quantity = Quantity.Get(context),
            HoldTtl = HoldTtl.Get(context),
            ReservationDataTtl = ReservationDataTtl.Get(context),
            Database = Database.Get(context)
        };
        var result = await redisInventoryService.ReserveAsync(request, context.CancellationToken);

        Result.Set(context, result);
        await context.CompleteActivityAsync();
    }
}
