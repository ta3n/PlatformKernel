using Liberty.ApplicationShared.Cqrs.BaseQuery;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.RoomGroup;

public record RoomGroupGetDisplaySettingQuery(
    long Id
) : IQuerySingleBase<RoomGroupDetailDisplaySettingResponse>;
