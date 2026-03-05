using Liberty.ApplicationShared.Cqrs.BaseQuery;

namespace Liberty.Reservation.Site.WebAPI.Application.UserCases.Queries.Booking;

public record BookingGetFacilityQuery : IQuerySingleBase<BookingFacilityResponse>;
