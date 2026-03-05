using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.Manager.Application.Auth;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.BookingReservation;

public class BookingGetAllAuditLogQueryHandler(
    IMapper mapper,
    IMediator mediator,
    ISecurityContextAccessor securityContextAccessor,
    IReservationRepository reservationRepository
) : QueryPageBaseHandler<BookingGetAllAuditLogQuery, BookingAuditLogResponse>(mapper)
{
    protected override async Task<(IHeaderDictionary, IEnumerable<BookingAuditLogResponse>)> HandleAsync(
        BookingGetAllAuditLogQuery request,
        CancellationToken cancellationToken
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        var existingReservation = await reservationRepository
            .GetQueryableWithAsNoTracking()
            .Where(
                x => x.Id == request.Id && x.Facility!.Id == facilityId
            )
            .AnyAsync(cancellationToken);
        if (!existingReservation)
        {
            throw new ReservationNotfoundException();
        }

        var (_, auditLogs) = await mediator.Send(
            new Reservation.Application.UseCases.Queries.BookingReservation.BookingGetAllAuditLogQuery(request.Id, request.Pageable),
            cancellationToken
        );

        return (new HeaderDictionary(), auditLogs);
    }
}
