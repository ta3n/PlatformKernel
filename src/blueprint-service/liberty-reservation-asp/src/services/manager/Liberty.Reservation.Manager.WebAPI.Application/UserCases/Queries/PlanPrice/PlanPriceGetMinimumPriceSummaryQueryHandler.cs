using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.Facility;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.PlanPrice;

public class PlanPriceGetMinimumPriceSummaryQueryHandler(
    IMapper mapper,
    IMediator mediator
) : QuerySingleBaseHandler<PlanPriceGetMinimumPriceSummaryQuery, PlanRoomSiteMinimumPriceSummaryResponse>(mapper)
{
    protected override async Task<(IHeaderDictionary, PlanRoomSiteMinimumPriceSummaryResponse)> HandleAsync(
        PlanPriceGetMinimumPriceSummaryQuery request,
        CancellationToken cancellationToken
    )
    {
        var (_, facilityMinimumPrice) = await mediator.Send(new FacilityGetMinimumPriceQuery(), cancellationToken);

        var (_, planMinimumPrice) = await mediator.Send(
            new PlanPriceGetMinimumPriceQuery(request.PlanId, request.RoomId, request.SiteId),
            cancellationToken
        );

        var isEnableMinimumPrice = facilityMinimumPrice.IsEnabledMinimumPrice || planMinimumPrice.IsEnabledMinimumPrice;

        int? minimumPrice = null;

        if (planMinimumPrice.IsEnabledMinimumPrice)
        {
            minimumPrice = planMinimumPrice.MinimumPrice;
        }
        else if (facilityMinimumPrice.IsEnabledMinimumPrice)
        {
            minimumPrice = facilityMinimumPrice.MinimumPrice;
        }

        var response = new PlanRoomSiteMinimumPriceSummaryResponse
        {
            FacilityMinimumPrice = facilityMinimumPrice,
            PlanMinimumPrice = planMinimumPrice,
            IsEnableMinimumPrice = isEnableMinimumPrice,
            MinimumPrice = minimumPrice
        };
        return (new HeaderDictionary(), response);
    }
}
