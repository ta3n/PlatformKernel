using SharedKernel.IntegrationEvent;
using SharedKernel.IntegrationEvent.Events;
using SharedKernel.IntegrationEvent.Models;

namespace SharedKernel.IntegrationEvent.Test;

public class UnitTest1
{
    [Fact]
    public void ImageResizeEvent_SetsExpectedEventMetadata()
    {
        var @event = new ImageResizeEvent();

        Assert.Equal(nameof(ImageResizeEvent), @event.EventName);
        Assert.Equal("ScheduleJobEvent", @event.EventType);
        Assert.NotEqual(Guid.Empty, @event.CorrelationId);
    }

    [Fact]
    public void SerializeJsonData_RoundTripsPayload()
    {
        var payload = new ImageResizeModel("origin", "thumb", "small");
        var @event = new ImageResizeEvent();

        @event.SerializeJsonData(payload);
        var restored = @event.DeserializeJsonData<ImageResizeModel>();

        Assert.NotNull(restored);
        Assert.Equal(payload, restored);
    }

    [Fact]
    public void AppQueues_ExposeStableQueueNames()
    {
        Assert.Equal("BookingAggregationAuditLogQueue", AppQueues.BookingAggregationAuditLogQueue);
        Assert.Equal("BookingAuditLogPartitionQueue", AppQueues.BookingAuditLogPartitionQueue);
    }
}
