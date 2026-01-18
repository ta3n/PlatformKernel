using Liberty.Reservation.Manager.Application.Models;

namespace Liberty.Reservation.Booking.Worker.Application.UserCases.Commands.BookingSearch;

public record BookingSearchPrecomputePartitionCommand : ActionCommandBase<
    FacilitySiteFlatAvailableModel,
    (string PartitionKey, bool IsSuccess)
>;
