using Liberty.ApplicationShared.Cqrs.BaseQuery;
using Liberty.Reservation.Application.Behaviors;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Responses;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.UserCases.Queries.Kakusan;

[IgnoreValidation]
public record KakusanGetAllBookingQuery(
    GetBookingRequest Request
) : IQuerySingleBase<GetBookingResponse>;
