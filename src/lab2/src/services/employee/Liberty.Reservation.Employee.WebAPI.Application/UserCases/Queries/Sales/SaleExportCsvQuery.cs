using Liberty.ApplicationShared.Cqrs.BaseQuery;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.Sales;

public record SaleExportCsvQuery(
    SaleGetAllRequest Request
) : IQueryBase<BookingReservationExportCsvStreamResponse>;
