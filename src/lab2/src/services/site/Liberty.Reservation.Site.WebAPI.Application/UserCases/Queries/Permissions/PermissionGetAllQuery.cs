using Liberty.ApplicationShared.Cqrs.BaseQuery;

namespace Liberty.Reservation.Site.WebAPI.Application.UserCases.Queries.Permissions;

public record PermissionGetAllQuery : IQuerySingleBase<IEnumerable<PermissionResponse>>;
