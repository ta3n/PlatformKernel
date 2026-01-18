namespace Liberty.Reservation.Application.AuditEvents;

public interface IBookingAggregateEvent : IAuditEventData
{
    long Id { get; }
    string AggregateCode { get; }
    object? Request { get; }
    string? UserCode { get; }
    long FacilityId { get; set; }
    long SiteId { get; set; }
    long? OldId { get; set; }
    string? LanguageCode { get; set; }

    string BuildJobSuffix(
        long aggregateId
    );

    TimeSpan? GetScheduleDelay();
}
