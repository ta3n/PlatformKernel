namespace Liberty.Reservation.Application.Models;

public record BookingSearchQueryableFilterParameter(
    long FacilityId,
    long SiteId,
    long ReservationDate,
    int RestNumber,
    TimeSpan TimeZone,
    string? Secret,
    bool ISecret = false,
    long[]? PlanIds = null,
    long[]? RoomGroupIds = null,
    bool? DayUse = false
);
