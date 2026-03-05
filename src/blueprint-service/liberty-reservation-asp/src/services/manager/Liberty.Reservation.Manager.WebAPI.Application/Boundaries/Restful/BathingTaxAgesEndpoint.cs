using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.BathingTaxAge;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.BathingTaxAge;

namespace Liberty.Reservation.Manager.WebAPI.Application.Boundaries.Restful;

[Route("api/bathing-tax-ages")]
public class BathingTaxAgesEndpoint(
    IMapper mapper,
    IMediator mediator,
    IPersonAgeTypeService personAgeTypeService,
    ICacheManagementService cacheManagementService
) : BaseEndpoint(mapper, mediator)
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateBathingTaxAgeAsync(
        [FromBody] BathingTaxAgeCreateRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new BathingTaxAgeCreateCommand { Payload = request },
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response);
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateBathingTaxAgeAsync(
        [FromBody] BathingTaxAgeUpdateRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new BathingTaxAgeUpdateCommand { Payload = request },
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response);
    }

    [HttpPatch("change-status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ChangeStatusBathingTaxAgeAsync(
        [FromBody] BathingTaxAgeChangeStatusRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new BathingTaxAgeChangeStatusCommand { Payload = request },
            cancellationToken
        );
        return ActionResultUtil.WrapOrNotFound(response);
    }

    [HttpPatch("change-facility-setting")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AdjustFacilitySettingAsync(
        [FromBody] BathingTaxAgeChangeSettingFacilityRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new BathingTaxAgeChangeSettingFacilityCommand { Payload = request },
            cancellationToken
        );
        return ActionResultUtil.WrapOrNotFound(response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(BathingTaxAgeResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllBathingTaxAgesAsync(
        IPageable pageable,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new BathingTaxAgeGetAllQuery(pageable),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpPatch("visible")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ChangeVisibleTaxAgeAsync(
        [FromBody] BathingTaxAgeVisibleRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new BathingTaxAgeVisibleCommand { Payload = request },
            cancellationToken
        );
        return ActionResultUtil.WrapOrNotFound(response);
    }

    [HttpPut("order")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public virtual async Task<IActionResult> ArrangeOrderAsync(
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

        await personAgeTypeService.ArrangeOrderAsync(
            request.Ids,
            cancellationToken
        );

        cacheManagementService.RemoveFacilityRelatedCache();

        return NoContent();
    }
}
