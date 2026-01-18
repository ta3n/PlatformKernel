using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Liberty.Pagination;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using MediatR;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.BookingReservation;

public class ReservationExportCsvQueryHandler(
    IMapper mapper,
    IMediator mediator
) : QueryBaseHandler<ReservationExportCsvQuery, BookingReservationExportCsvStreamResponse>(mapper)
{
    protected override Task<(IHeaderDictionary, BookingReservationExportCsvStreamResponse)> HandleAsync(
        ReservationExportCsvQuery request,
        CancellationToken cancellationToken
    )
    {
        var objectName = "reservation_list";
        var fileName = $"{objectName}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";
        var headers = new HeaderDictionary
        {
            { "Content-Type", "text/csv; charset=utf-8" },
            { "Content-Disposition", $"attachment; filename=\"{fileName}\"" },
            { "X-Content-Type-Options", "nosniff" }
        };

        return Task.FromResult<(IHeaderDictionary, BookingReservationExportCsvStreamResponse)>(
            (
                headers,
                new BookingReservationExportCsvStreamResponse
                {
                    WriteToStreamAsync = async (
                        stream,
                        token
                    ) =>
                    {
                        await using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);
                        await writer.WriteAsync(Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble()));
                        await using var csv = new CsvWriter(
                            writer,
                            new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true }
                        );

                        csv.Context.RegisterClassMap<BookingReservationCsvMap>();

                        csv.WriteHeader<BookingReservationCsv>();
                        await csv.NextRecordAsync();

                        var page = 1;
                        const int pageSize = 100;
                        bool hasMore;

                        do
                        {
                            var (_, content) = await mediator.Send(
                                new ReservationGetAllByFilterQuery(
                                    request.Request,
                                    Pageable.Of(page, pageSize)
                                ),
                                token
                            );

                            var items = content.ToList();
                            hasMore = items.Count == pageSize;
                            page++;

                            foreach (var itemCsv in items.Select(item => Mapper.Map<BookingReservationCsv>(item)))
                            {
                                csv.WriteRecord(itemCsv);
                                await csv.NextRecordAsync();
                            }

                            await writer.FlushAsync(token);
                        } while (hasMore && !token.IsCancellationRequested);
                    }
                }
            )
        );
    }
}
