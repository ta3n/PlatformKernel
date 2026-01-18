using Liberty.Pagination.Extensions;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.BookingReservation;

public class ReservationGetAllPersonAgeTypesQueryHandler(
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    IReservationRepository reservationRepository,
    IPersonAgeTypeRepository personAgeTypeRepository
) : QueryPageBaseHandler<ReservationGetAllPersonAgeTypesQuery, PersonAgeTypeResponse>(mapper)
{
    protected override async Task<(IHeaderDictionary, IEnumerable<PersonAgeTypeResponse>)> HandleAsync(
        ReservationGetAllPersonAgeTypesQuery request,
        CancellationToken cancellationToken
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        var existingReservation = await reservationRepository
                .GetQueryableWithAsNoTracking()
                .Where(
                    x => x.Id == request.Id && x.FacilityId == facilityId
                )
                .Select(
                    x => new
                    {
                        x.FacilityId,
                        x.PlanId,
                        x.SiteId,
                        x.RoomGroupId
                    }
                )
                .SingleOrDefaultAsync(
                    cancellationToken
                )
            ?? throw new ReservationNotfoundException();

        var queryable = personAgeTypeRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.IsEnabled && x.IsVisible)
            .Where(
                x =>
                    x.PlanRoomGroupSitePersonAgeTypes!.Any(
                        x => x.PlanId == existingReservation.PlanId
                            && x.SiteId == existingReservation.SiteId
                            && x.RoomGroupId == existingReservation.RoomGroupId
                            && x.IsEnabled
                    )
            )
            .Where(
                x => x.FacilityPersonAgeTypes!.Any(
                    t => t.FacilityId == facilityId
                )
            )
            .OrderBy(x => x.DisplayOrder)
            .ProjectTo<PersonAgeTypeResponse>(Mapper.ConfigurationProvider);

        var page = await queryable.UsePageableAsync(
            request.Pageable,
            cancellationToken: cancellationToken
        );

        var response = page.Content;
        var headers = page.GeneratePaginationHttpHeaders();

        return (headers, response);
    }
}
