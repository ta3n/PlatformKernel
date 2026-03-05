using Liberty.ApplicationShared.Cqrs.BaseQuery;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.Facility;

public record FacilityGetSeedStatusQuery(
    long Id
) : IQuerySingleBase<FacilitySeedStatusResponse>;
