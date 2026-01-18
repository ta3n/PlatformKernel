using Liberty.Pagination;
using Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful.Base;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.BookingReservation;
using Liberty.Reservation.Employee.WebAPI.Application.Web.Rest.Utilities;
using MediatR;

namespace Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful;

[Route("api/reservations")]
[ApiVersion(1)]
public class ReservationsEndpoint(
    IMapper mapper,
    IMediator mediator
) : BaseEndpoint(mapper, mediator)
{
    [HttpGet("filter-options")]
    [ProducesResponseType(typeof(ReservationFilterOptionsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFilterOptions()
    {
        var (headers, response) = await Mediator!.Send(
            new ReservationGetFilterOptionsQuery(
            )
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpPost("search")]
    [ProducesResponseType(typeof(List<BookingReservationResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchReservations(
        [FromQuery] IPageable pageable,
        [FromBody] ReservationGetAllBylFilterRequest request,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new ReservationGetAllByFilterQuery(
                request,
                pageable
            ),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("{id:long:min(1)}")]
    [ProducesResponseType(typeof(ReservationDetailResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReservationDetails(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new ReservationGetDetailsQuery(id),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpPost("export")]
    public async Task<IActionResult> ExportCsv(
        [FromBody] ReservationGetAllBylFilterRequest request,
        CancellationToken cancellationToken
    )
    {
        var (headers, result) = await Mediator!.Send(
            new ReservationExportCsvQuery(request),
            cancellationToken
        );

        foreach (var header in headers)
        {
            Response.Headers[header.Key] = header.Value;
        }

        await result.WriteToStreamAsync(Response.Body, cancellationToken);

        return new EmptyResult();
    }
}
