using Liberty.ApplicationShared.Cqrs.BaseQuery;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.Facility;

public record FacilityGetBasicSettingQuery(
) : IQuerySingleBase<FacilityDetailBasicSettingResponse>;
