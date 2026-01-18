using Liberty.Reservation.Application.AuditEvents;
using Liberty.Reservation.Application.Models;
using Liberty.SysIntegrationEvent.Events;
using MassTransit;
using Newtonsoft.Json;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Events.Booking;

public class BookingAggregateAuditHandler<T>(
    ILogger<BookingAggregateAuditHandler<T>> logger,
    IBus bus
)
    : INotificationHandler<T>
    where T : INotification, IBookingAggregateEvent
{
    protected List<IntegrationEventOutbox> EventOutboxes { get; } = [];

    public async Task Handle(
        T notification,
        CancellationToken cancellationToken
    )
    {
        var model = new BookingAuditLogModel(
            typeof(T).Name,
            notification.Id,
            notification.AggregateCode,
            JsonConvert.SerializeObject(notification.Request),
            notification.FacilityId,
            notification.SiteId,
            notification.UserCode,
            notification.OldId,
            notification.LanguageCode
        );

        var eventPayload = new BookingAggregateAuditLogEvent();
        eventPayload.SerializeJsonData(model);

        logger.LogInformation("Push queue event: {EventName}", eventPayload.EventName);

        await bus.Send(eventPayload, cancellationToken);
    }
}
