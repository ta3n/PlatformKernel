using Liberty.ApplicationShared.Cqrs.BaseQuery;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Responses;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.UserCases.Queries.Kakusan;

public record KakusanGetStatusQuery(
    string Code
) : IQuerySingleBase<RoomAdjustmentStatusResponse>;
