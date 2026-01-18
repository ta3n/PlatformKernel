using AutoMapper;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Application.Models.Responses;
using Microsoft.AspNetCore.Http;
using NodaTime;
using NodaTime.TimeZones;

namespace Liberty.Reservation.Application.UseCases.Queries.Timezone;

public class GetAllTimeZoneQueryHandler(
    IMapper mapper
) : QueryListBaseHandler<GetAllTimeZoneQuery, TimeZoneResponse>(mapper)
{
    protected override Task<(IHeaderDictionary, IEnumerable<TimeZoneResponse>)> HandleAsync(
        GetAllTimeZoneQuery request,
        CancellationToken cancellationToken
    )
    {
        var now = SystemClock.Instance.GetCurrentInstant();
        var source = TzdbDateTimeZoneSource.Default;

        var canonicalIds = source.ZoneLocations!
            .Select(z => z.ZoneId)
            .Distinct()
            .ToHashSet();

        var response = canonicalIds
            .Select(id =>
            {
                var zone = DateTimeZoneProviders.Tzdb[id];
                var offset = zone.GetUtcOffset(now).ToTimeSpan();
                return new TimeZoneResponse(id, offset);
            })
            .OrderBy(x => x.UtcOffset)
            .ThenBy(x => x.Name)
            .ToList();

        return Task.FromResult<(IHeaderDictionary, IEnumerable<TimeZoneResponse>)>(
            (
                new HeaderDictionary(),
                response
            )
        );
    }
}
