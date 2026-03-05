using Liberty.ApplicationShared.Cqrs.BaseQuery;

namespace Liberty.Reservation.Site.WebAPI.Application.UserCases.Queries.SystemConfig;

public record SystemConfigGetQuery : IQuerySingleBase<SystemConfigResponse>;
