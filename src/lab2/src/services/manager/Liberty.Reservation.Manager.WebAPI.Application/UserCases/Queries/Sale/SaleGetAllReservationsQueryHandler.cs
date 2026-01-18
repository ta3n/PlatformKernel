using Liberty.Pagination.Extensions;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.UnitOfWork.DbFunctions;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.Sale;

public class SaleGetAllReservationsQueryHandler(
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    IReservationRepository reservationRepository
) : QueryPageBaseHandler<SaleGetAllReservationsQuery, SaleDetailResponse>(mapper)
{
    private readonly ReservationStatus[] _allowedReservationStates =
    [
        ReservationStatus.Confirmed,
        ReservationStatus.Reserved,
        ReservationStatus.UserCanceled,
        ReservationStatus.GuestCanceled,
        ReservationStatus.ManagerCanceled,
        ReservationStatus.Modified
    ];

    protected override async Task<(IHeaderDictionary, IEnumerable<SaleDetailResponse>)> HandleAsync(
        SaleGetAllReservationsQuery request,
        CancellationToken cancellationToken
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        var queryable = reservationRepository
            .GetQueryableWithAsNoTracking()
            .Where(x => x.Facility!.Id == facilityId)
            .Where(x => x.CheckInDate.GetCheckOutDate(x.RestNumber) >= request.StartAppDateId)
            .Where(x => x.CheckInDate.GetCheckOutDate(x.RestNumber) <= request.EndAppDateId)
            .Where(x => _allowedReservationStates.Contains(x.ReservationState))
            .OrderByDescending(x => x.ReservationDateTime)
            .ProjectTo<SaleDetailResponse>(Mapper.ConfigurationProvider);

        var page = await queryable.UsePageableAsync(
            request.Pageable,
            false,
            cancellationToken
        );

        var headers = page.GeneratePaginationHttpHeaders();
        var data = page.Content;

        return (headers, data);
    }
}
