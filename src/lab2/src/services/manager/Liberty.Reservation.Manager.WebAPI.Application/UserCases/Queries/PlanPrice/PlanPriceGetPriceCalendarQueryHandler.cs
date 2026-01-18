using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Application.Utils;
using SqlKata;
using SqlKata.Execution;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.PlanPrice;

public class PlanPriceGetPriceCalendarQueryHandler(
    IMapper mapper,
    IUnitOfWork unitOfWork,
    QueryFactory queryFactory
) : QuerySingleBaseHandler<PlanPriceGetPriceCalendarQuery, PlanRoomDetailPriceCalendarResponse>(mapper)
{
    private readonly EntityProperty _planRoomGroupSiteAppDateProp = unitOfWork.GetEntityProperty<PlanRoomGroupSiteAppDate>();

    private readonly EntityProperty _planRoomGroupSiteAppDatePriceDataProp =
        unitOfWork.GetEntityProperty<PlanRoomGroupSiteAppDatePriceData>();

    private readonly EntityProperty _priceDataProp = unitOfWork.GetEntityProperty<PriceData>();

    protected override async Task<(IHeaderDictionary, PlanRoomDetailPriceCalendarResponse)> HandleAsync(
        PlanPriceGetPriceCalendarQuery request,
        CancellationToken cancellationToken
    )
    {
        var query = BuildSqlQuery(request);
        var dataOfPrice =
            await queryFactory.FromQuery(query)
                .GetAsync<RoomTypePriceDataCalendarResponse>(cancellationToken: cancellationToken);

        var roomTypePriceDataCalendarResponses = dataOfPrice as RoomTypePriceDataCalendarResponse[] ?? [.. dataOfPrice];
        var result = roomTypePriceDataCalendarResponses
            .GroupBy(x => new
                {
                    x.DateCalendar,
                    x.PersonMin,
                    x.PersonMax,
                }
            )
            .Select(x => new RoomTypePriceDataCalendarResponse
                {
                    PriceDataId = x.First().PriceDataId,
                    DateCalendar = x.First().DateCalendar,
                    PersonMin = x.First().PersonMin,
                    PersonMax = x.First().PersonMax,
                    Price = x.First().Price,
                    UseAutoDiscount = x.First().UseAutoDiscount
                }
            )
            .ToList();

        var data = new PlanRoomDetailPriceCalendarResponse(
            request.PlanId,
            request.RoomTypeId,
            request.SiteId,
            result
        );

        return (new HeaderDictionary(), data);
    }

    private Query BuildSqlQuery(
        PlanPriceGetPriceCalendarQuery request
    )
    {
        var query = new Query(_planRoomGroupSiteAppDateProp.TableName);
        query = InnerJoinQuery(query);
        query = ConditionQuery(query, request);
        query = SelectQuery(query);
        query = SortQuery(query);

        return query;
    }

    private Query InnerJoinQuery(
        Query query
    )
    {
        return query.Join(
                _planRoomGroupSiteAppDatePriceDataProp.TableName,
                o =>
                    o.On(
                            _planRoomGroupSiteAppDateProp.FullColumnName(nameof(PlanRoomGroupSiteAppDate.PlanId)),
                            _planRoomGroupSiteAppDatePriceDataProp.FullColumnName(nameof(PlanRoomGroupSiteAppDatePriceData.PlanId))
                        )
                        .On(
                            _planRoomGroupSiteAppDateProp.FullColumnName(nameof(PlanRoomGroupSiteAppDate.RoomGroupId)),
                            _planRoomGroupSiteAppDatePriceDataProp.FullColumnName(nameof(PlanRoomGroupSiteAppDatePriceData.RoomGroupId))
                        )
                        .On(
                            _planRoomGroupSiteAppDateProp.FullColumnName(nameof(PlanRoomGroupSiteAppDate.SiteId)),
                            _planRoomGroupSiteAppDatePriceDataProp.FullColumnName(nameof(PlanRoomGroupSiteAppDatePriceData.SiteId))
                        )
                        .On(
                            _planRoomGroupSiteAppDateProp.FullColumnName(nameof(PlanRoomGroupSiteAppDate.DateCalendar)),
                            _planRoomGroupSiteAppDatePriceDataProp.FullColumnName(nameof(PlanRoomGroupSiteAppDatePriceData.DateCalendar))
                        )
            )
            .Join(
                _priceDataProp.TableName,
                o =>
                    o.On(
                        _planRoomGroupSiteAppDatePriceDataProp.FullColumnName(nameof(PlanRoomGroupSiteAppDatePriceData.PriceDataId)),
                        _priceDataProp.FullColumnName(nameof(PriceData.Id))
                    )
            );
    }

    private Query ConditionQuery(
        Query query,
        PlanPriceGetPriceCalendarQuery request
    )
    {
        return query.Where(
                _planRoomGroupSiteAppDateProp.FullColumnName(nameof(PlanRoomGroupSiteAppDate.PlanId)),
                request.PlanId
            )
            .Where(
                _planRoomGroupSiteAppDateProp.FullColumnName(nameof(PlanRoomGroupSiteAppDate.RoomGroupId)),
                request.RoomTypeId
            )
            .Where(
                _planRoomGroupSiteAppDateProp.FullColumnName(nameof(PlanRoomGroupSiteAppDate.SiteId)),
                request.SiteId
            )
            .WhereBetween(
                _planRoomGroupSiteAppDateProp.FullColumnName(nameof(PlanRoomGroupSiteAppDate.DateCalendar)),
                request.StartDate,
                request.EndDate
            );
    }

    private static Query SortQuery(
        Query query
    )
    {
        return query
            .OrderBy(
                nameof(RoomTypePriceDataCalendarResponse.DateCalendar),
                nameof(RoomTypePriceDataCalendarResponse.PersonMin),
                nameof(RoomTypePriceDataCalendarResponse.PersonMax)
            )
            .OrderByDesc(nameof(RoomTypePriceDataCalendarResponse.PriceDataId));
    }

    private Query SelectQuery(
        Query query
    )
    {
        return query
            .Select(
                $"{_planRoomGroupSiteAppDatePriceDataProp.FullColumnName(nameof(PlanRoomGroupSiteAppDatePriceData.DateCalendar))} AS {nameof(RoomTypePriceDataCalendarResponse.DateCalendar)}",
                $"{_priceDataProp.FullColumnName(nameof(PriceData.PersonMax))} AS {nameof(RoomTypePriceDataCalendarResponse.PersonMax)}",
                $"{_priceDataProp.FullColumnName(nameof(PriceData.PersonMin))} AS {nameof(RoomTypePriceDataCalendarResponse.PersonMin)}",
                $"{_priceDataProp.FullColumnName(nameof(PriceData.Price))} AS {nameof(RoomTypePriceDataCalendarResponse.Price)}",
                $"{_planRoomGroupSiteAppDateProp.FullColumnName(nameof(PlanRoomGroupSiteAppDate.UseAutoDiscount))} AS {nameof(RoomTypePriceDataCalendarResponse.UseAutoDiscount)}",
                $"{_planRoomGroupSiteAppDatePriceDataProp.FullColumnName(nameof(PlanRoomGroupSiteAppDatePriceData.PriceDataId))} AS {nameof(RoomTypePriceDataCalendarResponse.PriceDataId)}"
            );
    }
}
