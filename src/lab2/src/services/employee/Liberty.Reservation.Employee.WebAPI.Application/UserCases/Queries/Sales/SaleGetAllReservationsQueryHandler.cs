using AutoMapper.QueryableExtensions;
using Liberty.Pagination.Extensions;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.WebAPI.Application.Web.Rest.Utilities;
using Liberty.UnitOfWork.DbFunctions;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.Sales;

public class SaleGetAllReservationsQueryHandler(
    IMapper mapper,
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
        var payload = request.Request;
        var queryable = reservationRepository
            .GetQueryableWithAsNoTracking()
            .Where(
                x =>
                x.CheckInDate
                    .GetDateTime(x.CheckInTime)
                    .AddHourOffset(x.Facility!.TimeZone) >= payload.StartAppDateId.GetDateTime(new TimeSpan(0, 0, 0))
            )
            .Where(
                x =>
                x.CheckInDate
                    .GetDateTime(x.CheckInTime)
                    .AddHourOffset(x.Facility!.TimeZone) <= payload.EndAppDateId.GetDateTime(new TimeSpan(23, 59, 59))
            )
            .Where(x => _allowedReservationStates.Contains(x.ReservationState))
            .Where(x => payload.FacilityIds.Contains(x.FacilityId))
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
