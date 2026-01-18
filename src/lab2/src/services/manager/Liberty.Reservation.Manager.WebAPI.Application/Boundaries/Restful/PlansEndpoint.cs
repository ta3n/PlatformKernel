using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.Models;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.Plan;

namespace Liberty.Reservation.Manager.WebAPI.Application.Boundaries.Restful;

[Route("api/plans")]
public class PlansEndpoint(
    IMapper mapper,
    IMediator mediator,
    IPlanService planService,
    ICacheManagementService cacheManagementService
) : BaseEndpoint(mapper, mediator)
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreatePlanAsync(
        [FromBody] PlanCreateRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new PlanCreateCommand { Payload = request },
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response)
            .WithHeaders(
                HeaderUtil.CreateEntityCreationAlert(
                    nameof(Plan),
                    response.ToString()
                )
            );
    }

    [HttpPut("order")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public virtual async Task<IActionResult> ArrangeOrderOfPlans(
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

        await planService.ArrangeOrderAsync(
            request.Ids,
            cancellationToken
        );

        cacheManagementService.RemoveFacilityRelatedCache();

        return NoContent();
    }

    [HttpPatch("{id:long:min(1)}/basic-setting")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdatePlanBasicSetting(
        [FromRoute] long id,
        [FromBody] PlanUpdateBasicSettingRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new PlanUpdateBasicSettingCommand(id) { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(Plan),
                    response.ToString()
                )
            );
    }

    [HttpPatch("{id:long:min(1)}/display")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdatePlanDisplay(
        [FromRoute] long id,
        [FromBody] PlanUpdateDisplayRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new PlanUpdateDisplayCommand(id) { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(Plan),
                    response.ToString()
                )
            );
    }

    [HttpPatch("{id:long:min(1)}/room-type")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdatePlanRoomType(
        [FromRoute] long id,
        [FromBody] PlanUpdateRoomTypeRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new PlanUpdateRoomTypeCommand(id) { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(Plan),
                    response.ToString()
                )
            );
    }

    [HttpPatch("{id:long:min(1)}/publish-accept")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdatePlanPublishAccept(
        [FromRoute] long id,
        [FromBody] PlanUpdatePublishAcceptRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new PlanUpdatePublishAcceptCommand(id) { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(Plan),
                    response.ToString()
                )
            );
    }

    [HttpPatch("{id:long:min(1)}/sale")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdatePlanSale(
        [FromRoute] long id,
        [FromBody] PlanUpdateSaleRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new PlanUpdateSaleCommand(id) { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(Plan),
                    response.ToString()
                )
            );
    }

    [HttpPatch("{id:long:min(1)}/payment-method")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdatePlanPaymentMethod(
        [FromRoute] long id,
        [FromBody] PlanUpdatePaymentMethodRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new PlanUpdatePaymentMethodCommand(id) { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(Plan),
                    response.ToString()
                )
            );
    }

    [HttpPatch("{id:long:min(1)}/meal")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdatePlanMeal(
        [FromRoute] long id,
        [FromBody] PlanUpdateMealRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new PlanUpdateMealCommand(id) { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(Plan),
                    response.ToString()
                )
            );
    }

    [HttpPatch("{id:long:min(1)}/option")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdatePlanOption(
        [FromRoute] long id,
        [FromBody] PlanUpdateOptionRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new PlanUpdateOptionCommand(id) { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(Plan),
                    response.ToString()
                )
            );
    }

    [HttpPatch("{id:long:min(1)}/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdatePlanCancel(
        [FromRoute] long id,
        [FromBody] PlanUpdateCancelRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new PlanUpdateCancelCommand(id) { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(Plan),
                    response.ToString()
                )
            );
    }

    [HttpPatch("{id:long:min(1)}/question")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdatePlanQuestion(
        [FromRoute] long id,
        [FromBody] PlanUpdateQuestionRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new PlanUpdateQuestionCommand(id) { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(Plan),
                    response.ToString()
                )
            );
    }

    [HttpPatch("{id:long:min(1)}/special")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdatePlanSpecial(
        [FromRoute] long id,
        [FromBody] PlanUpdateSpecialRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new PlanUpdateSpecialCommand(id) { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(Plan),
                    response.ToString()
                )
            );
    }

    [HttpPatch("{id:long:min(1)}/important-note")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdatePlanImportantNote(
        [FromRoute] long id,
        [FromBody] PlanUpdateImportantNoteRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new PlanUpdateImportantNoteCommand(id) { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(Plan),
                    response.ToString()
                )
            );
    }

    [HttpPatch("{id:long:min(1)}/enabled")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> PlanEnabled(
        [FromRoute] long id,
        [FromBody] PlanEnabledRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new PlanEnabledCommand(id) { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(Plan),
                    response.ToString()
                )
            );
    }

    [HttpPatch("{id:long:min(1)}/room-groups/{roomTypeId:long:min(1)}/enabled")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> EnabledPlanRoomType(
        [FromRoute] long id,
        [FromRoute] long roomTypeId,
        [FromBody] PlanEnabledRoomTypeRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new PlanEnabledRoomTypeCommand(id, roomTypeId) { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(Plan),
                    response.ToString()
                )
            );
    }

    [HttpDelete("{id:long:min(1)}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeletePlan(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new PlanDeleteCommand(id) { Payload = string.Empty },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityDeletionAlert(
                    nameof(Plan),
                    response.ToString()
                )
            );
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<PlanResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlans(
        IPageable pageable,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new PlanGetAllQuery(pageable),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("{id:long:min(1)}/basic-setting")]
    [ProducesResponseType(typeof(PlanBasicSettingResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> PlanGetBasicSetting(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new PlanGetGroupDetailsQuery(id, GroupOfPlan.BasicSetting),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("{id:long:min(1)}/display")]
    [ProducesResponseType(typeof(PlanDisplayResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlanDisplay(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new PlanGetGroupDetailsQuery(id, GroupOfPlan.Display),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("{id:long:min(1)}/room-type")]
    [ProducesResponseType(typeof(PlanRoomTypeResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlanRoomType(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new PlanGetGroupDetailsQuery(id, GroupOfPlan.RoomType),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("{id:long:min(1)}/publish-accept")]
    [ProducesResponseType(typeof(PlanPublishAcceptResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlanPublishAccept(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new PlanGetGroupDetailsQuery(id, GroupOfPlan.PublishAccept),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("{id:long:min(1)}/sale")]
    [ProducesResponseType(typeof(PlanSaleResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlanSale(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new PlanGetGroupDetailsQuery(id, GroupOfPlan.Sale),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("{id:long:min(1)}/payment-method")]
    [ProducesResponseType(typeof(PlanPaymentMethodResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlanPaymentMethod(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new PlanGetGroupDetailsQuery(id, GroupOfPlan.PaymentMethod),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("{id:long:min(1)}/meal")]
    [ProducesResponseType(typeof(PlanMealResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlanMeal(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new PlanGetGroupDetailsQuery(id, GroupOfPlan.Meal),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("{id:long:min(1)}/option")]
    [ProducesResponseType(typeof(PlanOptionResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlanOption(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new PlanGetGroupDetailsQuery(id, GroupOfPlan.Option),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("{id:long:min(1)}/cancel")]
    [ProducesResponseType(typeof(PlanCancelResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlanCancel(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new PlanGetGroupDetailsQuery(id, GroupOfPlan.Cancel),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("{id:long:min(1)}/question")]
    [ProducesResponseType(typeof(PlanQuestionResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlanQuestion(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new PlanGetGroupDetailsQuery(id, GroupOfPlan.Question),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("{id:long:min(1)}/special")]
    [ProducesResponseType(typeof(PlanSpecialResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlanSpecial(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new PlanGetGroupDetailsQuery(id, GroupOfPlan.Special),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("{id:long:min(1)}/important-note")]
    [ProducesResponseType(typeof(PlanImportantNoteResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlanImportantNote(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new PlanGetGroupDetailsQuery(id, GroupOfPlan.ImportantNote),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("{id:long:min(1)}/destinations")]
    [ProducesResponseType(typeof(List<SiteResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllDestinationOfPlan(
        [FromRoute] long id,
        IPageable pageable,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new PlanGetAllDestinationsQuery(id, pageable),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }
}
