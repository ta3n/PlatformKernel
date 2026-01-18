using System.Text.Json;
using Liberty.Reservation.Site.Public.WebAPI.Models.Requests;
using Liberty.Reservation.Site.Public.WebAPI.Models.Responses;

namespace Liberty.Reservation.Site.Public.WebAPI.Boundaries.Restful;

public partial class BookingEndpoint
{
    [HttpPost("search")]
    [ProducesResponseType(typeof(List<BookingSearchByPlanResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchBooking(
        [FromQuery] IPageable pageable,
        [FromBody] BookingSearchPlanRequest request,
        CancellationToken cancellationToken
    )
    {
        AdditionalIgnorePropertiesForResponse(
            "facilityId"
        );

        return await CallApiAsync(
            JsonSerializer.Serialize(request),
            cancellationToken
        );
    }

    [HttpGet("/plans/{planCode}/room-groups/{roomGroupCode}")]
    [ProducesResponseType(typeof(BookingDetailsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlanDetails(
       [FromRoute] string planCode,
       [FromRoute] string roomGroupCode,
       CancellationToken cancellationToken
    )
    {
        AdditionalIgnorePropertiesForResponse(
           "facilityId"
        );

        return await CallApiAsync(
             JsonSerializer.Serialize(
                 new GetPlanDetailRequest(planCode, roomGroupCode)),
             cancellationToken
         );
    }

    [HttpGet("/plans/{planCode}/room-groups/{roomGroupCode}/room")]
    [ProducesResponseType(typeof(BookingDetailsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRoomGroupDetail(
        [FromRoute] string planCode,
        [FromRoute] string roomGroupCode,
        CancellationToken cancellationToken
    )
    {
        AdditionalIgnorePropertiesForResponse(
           "facilityId"
        );
        return await CallApiAsync(
            JsonSerializer.Serialize(
                new GetPlanDetailRequest(planCode, roomGroupCode)),
            cancellationToken
        );
    }

    [HttpGet("/plans/{planCode}/room-groups/{roomGroupCode}/cancellation-policy")]
    [ProducesResponseType(typeof(BookingDetailsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCancellationPolicy(
        [FromRoute] string planCode,
        [FromRoute] string roomGroupCode,
        CancellationToken cancellationToken
    )
    {
        AdditionalIgnorePropertiesForResponse(
           "facilityId"
        );


        return await CallApiAsync(
            JsonSerializer.Serialize(
                new GetPlanDetailRequest(planCode, roomGroupCode)),
            cancellationToken
        );
    }

    [HttpGet("/plans/{planCode}/room-groups/{roomGroupCode}/special-notes")]
    [ProducesResponseType(typeof(BookingDetailsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSpecialNote(
        [FromRoute] string planCode,
        [FromRoute] string roomGroupCode,
        CancellationToken cancellationToken
    )
    {
        AdditionalIgnorePropertiesForResponse(
           "facilityId"
        );

        return await CallApiAsync(
            JsonSerializer.Serialize(
                new GetPlanDetailRequest(planCode, roomGroupCode)),
            cancellationToken
        );
    }
}
