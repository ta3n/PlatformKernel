using Liberty.ApplicationShared.Cqrs.BaseQuery;
using Liberty.Reservation.Application.Models.Responses;

namespace Liberty.Reservation.Application.UseCases.Queries.Timezone;

public record GetAllTimeZoneQuery : IQueryListBase<TimeZoneResponse>;
