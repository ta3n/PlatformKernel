using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Site.WebAPI.Application.UserCases.Queries.Booking;
using Microsoft.AspNetCore.Authorization;

namespace Liberty.Reservation.Site.WebAPI.Application.Boundaries.Restful;

[AllowAnonymous]
[Route("api/booking")]
public partial class BookingEndpoint(
    IMapper mapper,
    IMediator mediator,
    ISecurityContextAccessor securityContextAccessor,
    IPlanRoomGroupService planRoomGroupService,
    IServiceProvider serviceProvider
) : BaseEndpoint(mapper, mediator)
{
    [HttpGet("facility")]
    [ProducesResponseType(typeof(BookingFacilityResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFacilityBooking(
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new BookingGetFacilityQuery(),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpPost("price-calendars")]
    [ProducesResponseType(typeof(List<BookingCalendarPriceResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBookingRoomPriceCalendar(
        [FromBody] BookingSearchPriceCalendarRequest request,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new BookingGetAllPriceCalendarsQuery(
                request
            ),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }
}
