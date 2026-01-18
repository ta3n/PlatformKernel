using Liberty.Cache.Services;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.WebAPI.Application.Models;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroup;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.RoomGroup;

namespace Liberty.Reservation.Manager.WebAPI.Application.Boundaries.Restful;

[Route("api/room-groups")]
public class RoomGroupsEndpoint(
    IMapper mapper,
    IMediator mediator,
    IRoomGroupService roomGroupService,
    ICacheService cacheService,
    ISecurityContextAccessor securityContextAccessor,
    ICacheManagementService cacheManagementService
) : BaseEndpoint(mapper, mediator)
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateRoomGroup(
        [FromBody] RoomGroupCreateRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new RoomGroupCreateCommand { Payload = request },
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response)
            .WithHeaders(
                HeaderUtil.CreateEntityCreationAlert(
                    nameof(RoomGroup),
                    response.ToString()
                )
            );
    }

    [HttpPut("order")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public virtual async Task<IActionResult> ArrangeOrderOfRoomGroups(
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

        await roomGroupService.ArrangeOrderAsync(
            request.Ids,
            cancellationToken
        );

        cacheManagementService.RemoveFacilityRelatedCache();

        return NoContent();
    }

    [HttpPatch("{id:long:min(1)}/basic-configuration")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateBasicConfigurationOfRoomGroup(
        [FromRoute] long id,
        [FromBody] RoomGroupUpdateBasicConfigurationRequest request,
        CancellationToken cancellationToken
    )
    {
        request.Id = id;
        var response = await Mediator!.Send(
            new RoomGroupUpdateBasicConfigurationCommand { Payload = request },
            cancellationToken
        );

        var facilityId = securityContextAccessor.FacilityKey;

        await cacheService.ResetAsync(
            string.Format(CacheKeys.ResetPatternSiteBookingSearch, facilityId),
            false,
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(RoomGroup),
                    response.ToString()
                )
            );
    }

    [HttpPatch("{id:long:min(1)}/publication-setting")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdatePublicationSettingOfRoomGroup(
        [FromRoute] long id,
        [FromBody] RoomGroupUpdatePublicationSettingRequest request,
        CancellationToken cancellationToken
    )
    {
        request.Id = id;
        var response = await Mediator!.Send(
            new RoomGroupUpdatePublicationSettingCommand { Payload = request },
            cancellationToken
        );

        var facilityId = securityContextAccessor.FacilityKey;

        await cacheService.ResetAsync(
            string.Format(CacheKeys.ResetPatternSiteBookingSearch, facilityId),
            false,
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(RoomGroup),
                    response.ToString()
                )
            );
    }

    [HttpPatch("{id:long:min(1)}/display-setting")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateDisplaySettingOfRoomGroup(
        [FromRoute] long id,
        [FromBody] RoomGroupUpdateDisplaySettingRequest request,
        CancellationToken cancellationToken
    )
    {
        request.Id = id;
        var response = await Mediator!.Send(
            new RoomGroupUpdateDisplaySettingCommand { Payload = request },
            cancellationToken
        );

        var facilityId = securityContextAccessor.FacilityKey;

        await cacheService.ResetAsync(
            string.Format(CacheKeys.ResetPatternSiteBookingSearch, facilityId),
            false,
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(RoomGroup),
                    response.ToString()
                )
            );
    }

    [HttpPatch("{id:long:min(1)}/enable")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> EnableRoomGroup(
        [FromRoute] long id,
        [FromBody] RoomGroupEnabledRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new RoomGroupEnableCommand(id) { Payload = request },
            cancellationToken
        );

        var facilityId = securityContextAccessor.FacilityKey;

        await cacheService.ResetAsync(
            string.Format(CacheKeys.ResetPatternSiteBookingSearch, facilityId),
            false,
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(RoomGroup),
                    response.ToString()
                )
            );
    }

    [HttpDelete("{id:long:min(1)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteRoomGroup(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new RoomGroupDeleteCommand { Payload = new RoomGroupDeleteRequest(id) },
            cancellationToken
        );

        var facilityId = securityContextAccessor.FacilityKey;

        await cacheService.ResetAsync(
            string.Format(CacheKeys.ResetPatternSiteBookingSearch, facilityId),
            false,
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityDeletionAlert(
                    nameof(RoomGroup),
                    response.ToString()
                )
            );
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<RoomGroupResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllRoomGroups(
        IPageable pageable,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new RoomGroupGetAllQuery(pageable),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("{id:long:min(1)}/basic-configuration")]
    [ProducesResponseType(typeof(RoomGroupDetailBasicConfigurationResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBasicConfigurationOfRoomGroup(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var (header, response) = await Mediator!.Send(
            new RoomGroupGetBasicConfigurationQuery(id),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(header);
    }

    [HttpGet("{id:long:min(1)}/publication-setting")]
    [ProducesResponseType(typeof(RoomGroupDetailPublicationSettingResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPublicationSettingOfRoomGroup(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var (header, response) = await Mediator!.Send(
            new RoomGroupGetPublicationSettingQuery(id),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(header);
    }

    [HttpGet("{id:long:min(1)}/display-setting")]
    [ProducesResponseType(typeof(RoomGroupDetailDisplaySettingResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDisplaySettingOfRoomGroup(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var (header, response) = await Mediator!.Send(
            new RoomGroupGetDisplaySettingQuery(id),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(header);
    }

    [HttpGet("{id:long:min(1)}/published-in")]
    [ProducesResponseType(typeof(SiteOfRoomGroupDetailPublicationSettingResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllPublishedInOfRoomGroup(
        IPageable pageable,
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var (header, response) = await Mediator!.Send(
            new RoomGroupGetAllPublishedInQuery(id, pageable),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(header);
    }

    [HttpGet("{id:long:min(1)}/sale")]
    [ProducesResponseType(typeof(RoomGroupSaleResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRoomGroupSale(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new RoomGroupPlanGetGroupDetailsQuery(id, GroupOfPlan.Sale),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("{id:long:min(1)}/payment-method")]
    [ProducesResponseType(typeof(RoomGroupPaymentMethodResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRoomGroupPaymentMethod(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new RoomGroupPlanGetGroupDetailsQuery(id, GroupOfPlan.PaymentMethod),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("{id:long:min(1)}/meal")]
    [ProducesResponseType(typeof(RoomGroupMealResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRoomGroupMeal(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new RoomGroupPlanGetGroupDetailsQuery(id, GroupOfPlan.Meal),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("{id:long:min(1)}/option")]
    [ProducesResponseType(typeof(RoomGroupOptionResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRoomGroupOption(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new RoomGroupPlanGetGroupDetailsQuery(id, GroupOfPlan.Option),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("{id:long:min(1)}/cancel")]
    [ProducesResponseType(typeof(RoomGroupCancelResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRoomGroupCancel(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new RoomGroupPlanGetGroupDetailsQuery(id, GroupOfPlan.Cancel),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("{id:long:min(1)}/question")]
    [ProducesResponseType(typeof(RoomGroupQuestionResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRoomGroupQuestion(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new RoomGroupPlanGetGroupDetailsQuery(id, GroupOfPlan.Question),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("{id:long:min(1)}/special")]
    [ProducesResponseType(typeof(RoomGroupSpecialResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRoomGroupSpecial(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new RoomGroupPlanGetGroupDetailsQuery(id, GroupOfPlan.Special),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("{id:long:min(1)}/important-note")]
    [ProducesResponseType(typeof(RoomGroupImportantNoteResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRoomGroupImportantNote(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new RoomGroupPlanGetGroupDetailsQuery(id, GroupOfPlan.ImportantNote),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpPatch("{id:long:min(1)}/sale")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdatePlanSale(
        [FromRoute] long id,
        [FromBody] RoomGroupUpdateSaleRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new RoomGroupUpdateSaleCommand(id) { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(RoomGroup),
                    response.ToString()
                )
            );
    }

    [HttpPatch("{id:long:min(1)}/payment-method")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdatePlanPaymentMethod(
        [FromRoute] long id,
        [FromBody] RoomGroupUpdatePaymentMethodRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new RoomGroupUpdatePaymentMethodCommand(id) { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(RoomGroup),
                    response.ToString()
                )
            );
    }

    [HttpPatch("{id:long:min(1)}/meal")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdatePlanMeal(
        [FromRoute] long id,
        [FromBody] RoomGroupUpdateMealRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new RoomGroupUpdateMealCommand(id) { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(RoomGroup),
                    response.ToString()
                )
            );
    }

    [HttpPatch("{id:long:min(1)}/option")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdatePlanOption(
        [FromRoute] long id,
        [FromBody] RoomGroupUpdateOptionRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new RoomGroupUpdateOptionCommand(id) { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(RoomGroup),
                    response.ToString()
                )
            );
    }

    [HttpPatch("{id:long:min(1)}/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdatePlanCancel(
        [FromRoute] long id,
        [FromBody] RoomGroupUpdateCancelRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new RoomGroupUpdateCancelCommand(id) { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(RoomGroup),
                    response.ToString()
                )
            );
    }

    [HttpPatch("{id:long:min(1)}/question")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdatePlanQuestion(
        [FromRoute] long id,
        [FromBody] RoomGroupUpdateQuestionRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new RoomGroupUpdateQuestionCommand(id) { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(RoomGroup),
                    response.ToString()
                )
            );
    }

    [HttpPatch("{id:long:min(1)}/special")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdatePlanSpecial(
        [FromRoute] long id,
        [FromBody] RoomGroupUpdateSpecialRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new RoomGroupUpdateSpecialCommand(id) { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(RoomGroup),
                    response.ToString()
                )
            );
    }

    [HttpPatch("{id:long:min(1)}/important-note")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdatePlanImportantNote(
        [FromRoute] long id,
        [FromBody] RoomGroupUpdateImportantNoteRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new RoomGroupUpdateImportantNoteCommand(id) { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(RoomGroup),
                    response.ToString()
                )
            );
    }
}
