using System.Globalization;
using AutoMapper;
using Liberty.Pagination.Utilities;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Models.Responses;
using Microsoft.AspNetCore.Http;

namespace Liberty.Reservation.Application.UseCases.Queries.BookingReservation;

public class BookingGetAllAuditLogQueryHandler(
    IMapper mapper,
    IBookingReservationRepository bookingReservationRepository,
    IBookingAuditCompareService bookingAuditCompareService
) : QueryPageBaseHandler<BookingGetAllAuditLogQuery, BookingAuditLogResponse>(mapper)
{
    protected override async Task<(IHeaderDictionary, IEnumerable<BookingAuditLogResponse>)> HandleAsync(
        BookingGetAllAuditLogQuery request,
        CancellationToken cancellationToken
    )
    {
        var bookingTree = await bookingAuditCompareService.GetAllBookingTreeAsync(
            request.BookingId,
            cancellationToken
        );
        if (bookingTree is { Count: > 0 })
        {
            bookingTree = [bookingTree[0]]; // Only keep the root booking
        }

        var bookingIds = bookingTree
            .SelectMany(x => new[] { x.Id, x.ParentId })
            .Where(id => id != null)
            .Distinct()
            .ToList();
        if (bookingIds is { Count: <= 1 })
        {
            return (new HeaderDictionary(), []);
        }

        var page = await bookingReservationRepository.GetAllHistoriesByMainAsync(
            bookingIds.Where(x => x != null).Cast<long>(),
            request.Pageable,
            cancellationToken
        );

        var headers = page.GeneratePaginationHttpHeaders();
        var reservations = page.Content.ToList();

        var auditLogs = new List<BookingAuditLogResponse>();

        foreach (var bookingTreeModel in bookingTree)
        {
            var bookingNewId = bookingTreeModel.Id;
            var bookingOldId = bookingTreeModel.ParentId;

            var bookingNew = reservations.Find(x => x.Id == bookingNewId);
            var bookingOld = reservations.Find(x => x.Id == bookingOldId);
            if (bookingNew is null || bookingOld is null)
            {
                continue;
            }

            var changeItems = bookingAuditCompareService.GetAllBookingAuditLogs(
                bookingNew,
                bookingOld
            );

            var bookingAuditLog = new BookingAuditLogResponse(
                bookingNew.LastModifiedDateTime,
                bookingNew.LastModifiedBy,
                changeItems
            )
            {
                ChangeDate = ConvertLongToDateTime(
                    bookingNew.LastModifiedDateTime,
                    DefaultValues.TimeZoneOffset
                )
            };
            auditLogs.Add(bookingAuditLog);
        }

        return (headers, auditLogs);
    }

    private static DateTime? ConvertLongToDateTime(
        long? longDateId,
        int? timeZoneOffset = null
    )
    {
        if (longDateId is null)
        {
            return null;
        }

        var dateStr = longDateId.Value.ToString("00000000000000000");
        var changeDate = DateTime.ParseExact(
            dateStr,
            "yyyyMMddHHmmssfff",
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal
        );

        if (timeZoneOffset is not null)
        {
            changeDate = changeDate.AddMinutes(timeZoneOffset.Value);
        }

        return changeDate;
    }
}
