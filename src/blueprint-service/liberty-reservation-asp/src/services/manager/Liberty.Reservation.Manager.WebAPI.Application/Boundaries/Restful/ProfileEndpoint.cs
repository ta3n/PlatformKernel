using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.Profile;

namespace Liberty.Reservation.Manager.WebAPI.Application.Boundaries.Restful;

[Route("api/profile")]
public class ProfileEndpoint(
    IMapper mapper,
    IMediator mediator
) : BaseEndpoint(mapper, mediator)
{
    [HttpGet("facilities")]
    [ProducesResponseType(typeof(IEnumerable<FacilityManagementResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllFacilities(
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new ProfileGetAllFacilityQuery(),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }
}
