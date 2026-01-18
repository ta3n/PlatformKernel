using Liberty.ApplicationShared.Cqrs.BaseQuery;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.RoomGroupPrice;

public record RoomGroupPriceGetPriceCalendarQuery(
    long RoomTypeId,
    long SiteId,
    long StartDate,
    long EndDate
) : IQuerySingleBase<PlanRoomDetailPriceCalendarResponse>;
