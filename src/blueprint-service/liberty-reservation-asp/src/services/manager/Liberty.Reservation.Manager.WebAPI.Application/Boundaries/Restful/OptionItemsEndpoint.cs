using Liberty.Cache.Services;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.OptionItem;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.OptionItem;

namespace Liberty.Reservation.Manager.WebAPI.Application.Boundaries.Restful;

[Route("api/option-items")]
public class OptionItemsEndpoint(
    IMapper mapper,
    IMediator mediator,
    IOptionItemService optionItemService,
    ICacheService cacheService,
    ISecurityContextAccessor securityContextAccessor,
    ICacheManagementService cacheManagementService
) : BaseEndpoint(mapper, mediator)
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateOptionItem(
        [FromBody] OptionItemCreateRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new OptionItemCreateCommand { Payload = request },
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response)
            .WithHeaders(
                HeaderUtil.CreateEntityCreationAlert(
                    nameof(OptionItem),
                    response.ToString()
                )
            );
    }

    [HttpPut("order")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public virtual async Task<IActionResult> ArrangeOrderOfOptionItems(
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

        await optionItemService.ArrangeOrderAsync(
            request.Ids,
            cancellationToken
        );

        cacheManagementService.RemoveFacilityRelatedCache();

        return NoContent();
    }

    [HttpPut("{id:long:min(1)}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateOptionItem(
        [FromRoute] long id,
        [FromBody] OptionItemUpdateRequest request,
        CancellationToken cancellationToken
    )
    {
        request.Id = id;
        var response = await Mediator!.Send(
            new OptionItemUpdateCommand { Payload = request },
            cancellationToken
        );

        var facilityId = securityContextAccessor.FacilityKey;

        await cacheService.ResetAsync(
            string.Format(CacheKeys.ResetPatternQuestionByFacilityId, facilityId),
            false,
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(OptionItem),
                    response.ToString()
                )
            );
    }

    [HttpPatch("{id:long:min(1)}/enable")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> EnableOptionItem(
        [FromRoute] long id,
        [FromBody] OptionItemEnabledRequest request,
        CancellationToken cancellationToken
    )
    {
        request.Id = id;
        var response = await optionItemService.EnableAsync(
            id,
            request.IsEnabled,
            cancellationToken: cancellationToken
        );

        var facilityId = securityContextAccessor.FacilityKey;

        await cacheService.ResetAsync(
            string.Format(CacheKeys.ResetPatternQuestionByFacilityId, facilityId),
            false,
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(OptionItem),
                    response.Id.ToString()
                )
            );
    }

    [HttpDelete("{id:long:min(1)}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> DeleteOptionItem(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new OptionItemDeleteCommand { Payload = new OptionItemDeleteRequest(id) },
            cancellationToken
        );

        var facilityId = securityContextAccessor.FacilityKey;

        await cacheService.ResetAsync(
            string.Format(CacheKeys.ResetPatternQuestionByFacilityId, facilityId),
            false,
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityDeletionAlert(
                    nameof(OptionItem),
                    response.ToString()
                )
            );
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<OptionItemResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllOptionItems(
        IPageable pageable,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new OptionItemGetAllQuery(pageable),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("{id:long:min(1)}")]
    [ProducesResponseType(typeof(OptionItemDetailResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOptionItem(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var (header, response) = await Mediator!.Send(
            new OptionItemGetQuery(id),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(header);
    }
}
