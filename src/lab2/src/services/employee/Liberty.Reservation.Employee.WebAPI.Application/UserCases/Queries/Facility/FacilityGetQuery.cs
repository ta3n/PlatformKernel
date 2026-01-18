using Liberty.ApplicationShared.Cqrs.BaseQuery;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.Facility;

public record FacilityGetQuery(
    long Id
) : IQuerySingleBase<FacilityDetailResponse>;
