using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.CancellationPolicy;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.CancellationPolicy;

namespace Liberty.Reservation.Manager.WebAPI.Application.Boundaries.Restful;

[Route("api/cancellation-policies")]
public class CancellationPoliciesEndpoint(
    IMapper mapper,
    IMediator mediator,
    ICancellationService cancellationService,
    ICacheManagementService cacheManagementService
) : BaseEndpoint(mapper, mediator)
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateCancellationPolicy(
        [FromBody] CancellationPolicyCreateRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new CancellationPolicyCreateCommand { Payload = request },
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response)
            .WithHeaders(
                HeaderUtil.CreateEntityCreationAlert(
                    nameof(Cancellation),
                    response.ToString()
                )
            );
    }

    [HttpPut("order")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public virtual async Task<IActionResult> ArrangeOrderOfCancellationPolicies(
        [FromBody] ItemUpdateOrderRequest request,
        CancellationToken cancellationToken
    )
    {
        var validation = await new ItemUpdateArrangeOrderRequestValidator().ValidateAsync(
            request,
            cancellationToken
        );
        if (!validation.IsValid)
        {
            throw new AppRequestInvalidException(
                validation.GetErrorCode(),
                validation.GetErrorMessage(),
                validation.GetErrorField()
            );
        }

        await cancellationService.ArrangeOrderAsync(
            request.Ids,
            cancellationToken
        );

        cacheManagementService.RemoveFacilityRelatedCache();

        return NoContent();
    }

    [HttpPut("{id:long:min(1)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateCancellationPolicy(
        [FromRoute] long id,
        [FromBody] CancellationPolicyUpdateRequest request,
        CancellationToken cancellationToken
    )
    {
        request.Id = id;
        var response = await Mediator!.Send(
            new CancellationPolicyUpdateCommand { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(Cancellation),
                    response.ToString()
                )
            );
    }

    [HttpPatch("{id:long:min(1)}/enable")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> EnableCancellationPolicy(
        [FromRoute] long id,
        [FromBody] CancellationPolicyEnabledRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new CancellationPolicyEnableCommand(id) { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(Cancellation),
                    response.ToString()
                )
            );
    }

    [HttpDelete("{id:long:min(1)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteCancellationPolicy(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new CancellationPolicyDeleteCommand { Payload = new CancellationPolicyDeleteRequest(id) },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityDeletionAlert(
                    nameof(Cancellation),
                    response.ToString()
                )
            );
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<CancellationPolicyResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllCancellationPolicies(
        IPageable pageable,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new CancellationPolicyGetAllQuery(pageable),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("{id:long:min(1)}")]
    [ProducesResponseType(typeof(CancellationPolicyDetailResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCancellationPolicy(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var (header, response) = await Mediator!.Send(
            new CancellationPolicyGetQuery(id),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(header);
    }
}
