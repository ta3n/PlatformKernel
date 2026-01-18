using Liberty.Reservation.Application.AuditEvents;

namespace Liberty.Reservation.Site.WebAPI.Application.UserCases.Events.Booking;

public sealed class BookingCreatedEvent : IBookingAggregateEvent
{
    public long Id { get; init; }
    public string AggregateCode { get; init; } = null!;
    public object? Request { get; init; }
    public string? UserCode { get; init; }
    public long FacilityId { get; set; }
    public long SiteId { get; set; }
    public long? OldId { get; set; }
    public string? LanguageCode { get; set; }

    public string BuildJobSuffix(
        long aggregateId
    )
    {
        return $"Create_Reservation_{aggregateId}";
    }

    public TimeSpan? GetScheduleDelay()
    {
        return TimeSpan.FromSeconds(1);
    }
}
