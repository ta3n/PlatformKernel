using Liberty.ApplicationShared.Cqrs.BaseQuery;
using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Application.Models.Responses;

namespace Liberty.Reservation.User.WebAPI.Application.UserCases.Queries.Permissions;

public record PermissionGetAllQuery : IQuerySingleBase<IEnumerable<PermissionResponse>>;
