using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.PlanPrice;

public class PlanPriceGetPriceCalendarQueryHandler(
    IMapper mapper,
    IPlanRoomGroupSiteAppDateRepository appDateRepository,
    IPlanRoomGroupSiteAppDatePriceDataRepository siteAppDatePriceDataRepository
) : QuerySingleBaseHandler<PlanPriceGetPriceCalendarQuery, PlanRoomDetailPriceCalendarResponse>(mapper)
{
    protected override async Task<(IHeaderDictionary, PlanRoomDetailPriceCalendarResponse)> HandleAsync(
        PlanPriceGetPriceCalendarQuery request,
        CancellationToken cancellationToken
    )
    {
        var dataOfPrinceQueryable = siteAppDatePriceDataRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.PlanId == request.PlanId)
            .Where(x => x.RoomGroupId == request.RoomTypeId)
            .Where(x => x.SiteId == request.SiteId)
            .Where(
                x => x.DateCalendar >= request.StartDate
                    && x.DateCalendar <= request.EndDate
            )
            .ProjectTo<RoomTypePriceDataCalendarResponse>(Mapper.ConfigurationProvider);

        var dataOfPrince = await dataOfPrinceQueryable.ToListAsync(
            cancellationToken
        );

        var dataOfCalendarQueryable = appDateRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.PlanId == request.PlanId)
            .Where(x => x.RoomGroupId == request.RoomTypeId)
            .Where(x => x.SiteId == request.SiteId)
            .Where(
                x => x.DateCalendar >= request.StartDate
                    && x.DateCalendar <= request.EndDate
            )
            .ProjectTo<RoomTypePriceCalendarDataResponse>(Mapper.ConfigurationProvider);

        var dataOfCalendar = await dataOfCalendarQueryable.ToListAsync(
            cancellationToken
        );

        foreach (var item in dataOfPrince)
        {
            item.UseAutoDiscount = dataOfCalendar.Find(
                        x => x.DateCalendar == item.DateCalendar
                    )
                    ?.UseAutoDiscount
                ?? false;
        }

        dataOfPrince = [.. dataOfPrince.OrderBy(x => x.DateCalendar).ThenBy(x => x.PersonMin)];

        var data = new PlanRoomDetailPriceCalendarResponse(
            request.PlanId,
            request.RoomTypeId,
            request.SiteId,
            dataOfPrince
        );

        return (new HeaderDictionary(), data);
    }
}
