using Liberty.ApplicationShared.Cqrs.BaseQuery;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.PlanPrice;

public record PlanPriceGetPriceCalendarQuery(
    long PlanId,
    long RoomTypeId,
    long SiteId,
    long StartDate,
    long EndDate
) : IQuerySingleBase<PlanRoomDetailPriceCalendarResponse>;
