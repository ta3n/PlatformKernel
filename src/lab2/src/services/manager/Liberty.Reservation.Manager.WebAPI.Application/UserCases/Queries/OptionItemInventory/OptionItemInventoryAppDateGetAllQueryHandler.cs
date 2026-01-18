using Liberty.Pagination.Extensions;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.OptionItemInventory;

public class OptionItemInventoryAppDateGetAllQueryHandler(
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    IOptionItemRepository optionItemRepository
) : QueryPageBaseHandler<OptionItemInventoryAppDateGetAllQuery, OptionItemWithAppDatesResponse>(mapper)
{
    protected override async Task<(IHeaderDictionary, IEnumerable<OptionItemWithAppDatesResponse>)> HandleAsync(
        OptionItemInventoryAppDateGetAllQuery request,
        CancellationToken cancellationToken
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        var queryable = optionItemRepository.GetQueryableWithAsNoTracking()
            .Where(o => o.FacilityOptionItems!.Any(f => f.FacilityId == facilityId))
            .Select(
                o =>
                    new OptionItemWithAppDatesResponse(
                        o.Id,
                        o.Name!.GetValueByCode(DefaultValues.LanguageCode),
                        o.BaseNumber,
                        o.OptionItemAppDates!
                            .Where(d => d.AppDateId >= request.StartAppDate && d.AppDateId <= request.EndAppDate)
                            .Select(
                                d =>
                                    new OptionItemAppDateDetailResponse(
                                        d.AppDateId,
                                        d.SellNumber,
                                        d.OptionItem!.ReservationRoomGroupAppDateOptionItems!
                                            .Where(x => x.BookingDateId == d.AppDateId)
                                            .Where(
                                                x =>
                                                    x.Reservation!.ReservationState == ReservationStatus.Confirmed
                                                    || x.Reservation!.ReservationState == ReservationStatus.Reserved
                                                    || x.Reservation!.ReservationState == ReservationStatus.Modified
                                            )
                                            .Sum(x => x.Number),
                                        d.SellNumber
                                        - d.OptionItem!.ReservationRoomGroupAppDateOptionItems!
                                            .Where(x => x.BookingDateId == d.AppDateId)
                                            .Where(
                                                x =>
                                                    x.Reservation!.ReservationState == ReservationStatus.Confirmed
                                                    || x.Reservation!.ReservationState == ReservationStatus.Reserved
                                                    || x.Reservation!.ReservationState == ReservationStatus.Modified
                                            )
                                            .Sum(x => x.Number),
                                        d.IsNotSelled
                                    )
                            )
                            .ToList()
                    )
            );

        var page = await queryable.UsePageableAsync(
            request.Pageable,
            cancellationToken: cancellationToken
        );

        var headers = page.GeneratePaginationHttpHeaders();
        var data = page.Content.ToList();

        return (headers, data);
    }
}
