using Liberty.ApplicationShared.Cqrs.BaseQuery;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.SystemConfig;

public record SystemConfigGetQuery : IQuerySingleBase<SystemConfigResponse>;
