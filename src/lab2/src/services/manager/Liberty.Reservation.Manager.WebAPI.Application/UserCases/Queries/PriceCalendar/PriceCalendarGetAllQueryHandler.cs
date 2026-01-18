using Liberty.Pagination.Extensions;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.PriceCalendar;

public class PriceCalendarGetAllQueryHandler(
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    ICalendarAppDateAppDateTypeRepository calendarAppDateAppDateTypeRepository
) : QueryPageBaseHandler<PriceCalendarGetAllQuery, PriceCalendarResponse>(mapper)
{
    protected override async Task<(IHeaderDictionary, IEnumerable<PriceCalendarResponse>)> HandleAsync(
        PriceCalendarGetAllQuery request,
        CancellationToken cancellationToken
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        var queryable = calendarAppDateAppDateTypeRepository
            .GetQueryableWithAsNoTracking()
            .Where(
                x => x.Calendar!.FacilityCalendars!.Any(
                    t => t.FacilityId == facilityId
                )
            )
            .Where(
                x => x.AppDateType!.FacilityAppDateTypes!.Any(
                    t => t.FacilityId == facilityId
                )
            )
            .Where(
                x => x.DateCalendar >= request.StartDate
                    && x.DateCalendar <= request.EndDate
            )
            .Select(
                x => new PriceCalendarResponse(
                    x.AppDateTypeId,
                    x.DateCalendar,
                    x.AppDateType!.Name,
                    x.AppDateType!.ShortName,
                    x.AppDateType.Color,
                    x.AppDateType!.DisplayOrder,
                    x.AppDateType!.IsEnabled
                )
            );

        var page = await queryable.UsePageableAsync(
            request.Pageable,
            cancellationToken: cancellationToken
        );

        var data = page.Content;
        var headers = page.GeneratePaginationHttpHeaders();

        return (headers, data);
    }
}
