using Liberty.ApplicationShared.Cqrs.BaseQuery;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.Destination;

public record DestinationGetQuery(
    long Id
) : IQuerySingleBase<DestinationDetailResponse>;
