using Liberty.GmoPaymentGateway.Models;
using Liberty.GmoPaymentGateway.Services;
using MediatR;
using Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful.Base;
using Liberty.Pagination;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.BookingReservation;
using Liberty.Reservation.Employee.WebAPI.Application.Web.Rest.Utilities;

namespace Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful;

[Route("api/gmo-info")]
public class GmoInfoEndpoint(
    IGmoErrorCodeService gmoErrorCodeService,
    IMapper mapper,
    IMediator mediator
) : BaseEndpoint(mapper, mediator)
{
    [HttpGet("errors")]
    [ProducesResponseType(typeof(List<GmoErrorItem>), StatusCodes.Status200OK)]
    public IActionResult GetAllErrorCodes()
    {
        var allErrorCodes = gmoErrorCodeService.GetAll();
        return Ok(allErrorCodes);
    }

    [HttpPost("result")]
    [ProducesResponseType(typeof(List<GmoPaymentResultResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchGmoPayments(
        [FromQuery] IPageable pageable,
        CancellationToken cancellationToken
    )
    {
        var query = new ReservationGetAllGmoInfoQuery(pageable);

        var (headers, response) = await Mediator!.Send(query, cancellationToken);

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }
}
