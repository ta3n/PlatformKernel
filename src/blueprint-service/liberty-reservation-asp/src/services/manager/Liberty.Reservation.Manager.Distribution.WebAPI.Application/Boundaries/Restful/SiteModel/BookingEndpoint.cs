using Liberty.Protobuf.Site.V1;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Boundaries.Restful.Base;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Web.Extensions;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Web.Rest.Utilities;
using Microsoft.AspNetCore.Authorization;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Boundaries.Restful.SiteModel;

[AllowAnonymous]
[ApiExplorerSettings(GroupName = "site-module")]
[Route("api/booking")]
public class BookingEndpoint(
    IMapper mapper,
    IMediator mediator,
    SiteProtoEndpoint.SiteProtoEndpointClient siteProtoEndpointClient
) : BaseEndpoint(mapper, mediator)
{
    [HttpGet("facility")]
    [ProducesResponseType(typeof(GetFacilitySiteProtoReply), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFacilityBooking(
        [FromQuery] GetFacilitySiteProtoRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await siteProtoEndpointClient.GetFacilitySiteAsync(
            request,
            cancellationToken: cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response);
    }

    [HttpPost("search-by-plan")]
    [ProducesResponseType(typeof(List<BookingSearchByPlanProtoModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchBooking(
        [FromQuery] IPageable pageable,
        [FromBody] BookingSearchProtoModel request,
        CancellationToken cancellationToken
    )
    {
        var protoRequest = new SearchBookingByPlanProtoRequest
        {
            Pageable = new()
            {
                PageSize = pageable.PageSize,
                PageNumber = pageable.PageNumber,
                IsEnabled = pageable.IsEnabled
            },
            BookingSearch = request
        };

        // Send gRPC request
        var protoReply = await siteProtoEndpointClient.SearchBookingByPlanAsync(
            protoRequest,
            cancellationToken: cancellationToken
        );

        var header = new HeaderDictionary(
            protoReply.Headers.ToDictionary(
                x => x.Key,
                x => (Microsoft.Extensions.Primitives.StringValues)x.Value
            )
        );

        var response = protoReply.Data;

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(header);
    }
}
