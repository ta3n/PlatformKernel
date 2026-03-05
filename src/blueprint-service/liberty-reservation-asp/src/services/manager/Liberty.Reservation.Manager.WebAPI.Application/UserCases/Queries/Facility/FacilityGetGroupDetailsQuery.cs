using Liberty.ApplicationShared.Cqrs.BaseQuery;
using Liberty.Reservation.Manager.WebAPI.Application.Models;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.Facility;

public record FacilityGetGroupDetailsQuery(
    GroupOfFacility Group
) : IQuerySingleBase<object>;
