using Liberty.ApplicationShared.Cqrs.BaseQuery;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.BookingReservation;

public record ReservationExportCsvQuery(
    ReservationGetAllBylFilterRequest Request
) : IQueryBase<BookingReservationExportCsvStreamResponse>;
