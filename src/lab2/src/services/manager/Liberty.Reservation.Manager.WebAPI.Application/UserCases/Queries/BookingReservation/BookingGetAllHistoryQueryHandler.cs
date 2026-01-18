using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Events.Booking;
using Newtonsoft.Json;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.BookingReservation;

public class BookingGetAllHistoriesQueryHandler(
    IMapper mapper,
    IMediator mediator,
    IBookingAggregateAuditLogRepository bookingAggregateAuditLogRepository,
    ISecurityContextAccessor securityContextAccessor
) : QueryPageBaseHandler<BookingGetAllHistoriesQuery, BookingAuditLogResponse>(mapper)
{
    protected override async Task<(IHeaderDictionary, IEnumerable<BookingAuditLogResponse>)> HandleAsync(
        BookingGetAllHistoriesQuery request,
        CancellationToken cancellationToken
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;

        var queryableAuditLogs = bookingAggregateAuditLogRepository
            .GetQueryableWithAsNoTracking()
            .Where(
                x => x.AggregateId == request.Id
                    && x.FacilityId == facilityId
            )
            .Where(x => x.EventType == nameof(BookingUpdatedEvent))
            .OrderByDescending(x => x.Id)
            .Select(x => x.ChangedFields);

        var auditLogs = await queryableAuditLogs.FirstOrDefaultAsync(cancellationToken);

        if (string.IsNullOrEmpty(auditLogs))
        {
            var (_, auditLogResponses) = await mediator.Send(
                new Reservation.Application.UseCases.Queries.BookingReservation.BookingGetAllAuditLogQuery(request.Id, request.Pageable),
                cancellationToken
            );

            return (new HeaderDictionary(), auditLogResponses);
        }

        var response = (JsonConvert.DeserializeObject<IEnumerable<BookingAuditLogResponse>>(auditLogs) ?? [])
            .ToList();
        response = [.. response.Where(x => x.ChangeItems!.Any()).OrderByDescending(x => x.ChangeDate)];
        return (new HeaderDictionary(), response);
    }
}
