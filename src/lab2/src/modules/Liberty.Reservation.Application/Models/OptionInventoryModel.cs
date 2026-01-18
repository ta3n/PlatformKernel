using Liberty.Reservation.Application.Models.Responses;

namespace Liberty.Reservation.Application.Models;

public record OptionInventoryModel(
    long FacilityId,
    long SiteId,
    long PlanId,
    long CheckInDate,
    int RestNumber,
    List<OptionOfBookingSearchModel> Options,
    long? ExistingReservationId
);
