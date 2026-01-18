using Liberty.Reservation.Manager.WebAPI.Application.Models;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Facility;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.Facility;

namespace Liberty.Reservation.Manager.WebAPI.Application.Boundaries.Restful;

[Route("api/facility")]
public class FacilityEndpoint(
    IMapper mapper,
    IMediator mediator
) : BaseEndpoint(mapper, mediator)
{
    [HttpPatch("accept")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateFacilityAcceptanceAsync(
        [FromBody] FacilityUpdateAcceptRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new FacilityUpdateAcceptCommand { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(Facility),
                    response.ToString()
                )
            );
    }

    [HttpPatch("access")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateFacilityAccessAsync(
        [FromBody] FacilityUpdateAccessRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new FacilityUpdateAccessCommand { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(Facility),
                    response.ToString()
                )
            );
    }

    [HttpPatch("basic-setting")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateFacilityBasicSettingAsync(
        [FromBody] FacilityUpdateBasicSettingRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new FacilityUpdateBasicSettingCommand { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(Facility),
                    response.ToString()
                )
            );
    }

    [HttpPatch("bath")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateFacilityBathAsync(
        [FromBody] FacilityUpdateBathRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new FacilityUpdateBathCommand { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(Facility),
                    response.ToString()
                )
            );
    }

    [HttpPatch("classification")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateFacilityClassificationAsync(
        [FromBody] FacilityUpdateClassificationRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new FacilityUpdateClassificationCommand { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(Facility),
                    response.ToString()
                )
            );
    }

    [HttpPatch("payment-method")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateFacilityPaymentMethodAsync(
        [FromBody] FacilityUpdatePaymentMethodRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new FacilityUpdatePaymentMethodCommand { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(Facility),
                    response.ToString()
                )
            );
    }

    [HttpPatch("minimum-price")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateFacilityMinimumPriceAsync(
        [FromBody] FacilityUpdateMinimumPriceRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new FacilityUpdateMinimumPriceCommand { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(Facility),
                    response.ToString()
                )
            );
    }

    [HttpGet("minimum-price")]
    [ProducesResponseType(typeof(FacilityMinimumPriceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFacilityMinimumPriceAsync(
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new FacilityGetMinimumPriceQuery(),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpPatch("publish")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateFacilityPublishingAsync(
        [FromBody] FacilityUpdatePublicationInformationRequest publicationInformationRequest,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new FacilityUpdatePublicationInformationCommand { Payload = publicationInformationRequest },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(Facility),
                    response.ToString()
                )
            );
    }

    [HttpPatch("reservation-change")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateFacilityReservationChangeAsync(
        [FromBody] FacilityUpdateReservationChangeRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new FacilityUpdateReservationChangeCommand { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(Facility),
                    response.ToString()
                )
            );
    }

    [HttpPatch("reservation-setting")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateFacilityReservationSettingAsync(
        [FromBody] FacilityUpdateReservationSettingRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new FacilityUpdateReservationSettingCommand { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(Facility),
                    response.ToString()
                )
            );
    }

    [HttpGet("accept")]
    [ProducesResponseType(typeof(FacilityDetailAcceptResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAccept(
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new FacilityGetGroupDetailsQuery(GroupOfFacility.Accept),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("access")]
    [ProducesResponseType(typeof(FacilityDetailAccessResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAccess(
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new FacilityGetGroupDetailsQuery(GroupOfFacility.Access),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("basic-setting")]
    [ProducesResponseType(typeof(FacilityDetailBasicSettingResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBasicSetting(
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new FacilityGetBasicSettingQuery(),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("bath")]
    [ProducesResponseType(typeof(FacilityDetailBathResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBath(
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new FacilityGetGroupDetailsQuery(GroupOfFacility.Bath),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("classification")]
    [ProducesResponseType(typeof(FacilityDetailClassificationResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetClassification(
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new FacilityGetGroupDetailsQuery(GroupOfFacility.Classification),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("payment-method")]
    [ProducesResponseType(typeof(FacilityDetailPaymentMethodResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaymentMethod(
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new FacilityGetGroupDetailsQuery(GroupOfFacility.PaymentMethod),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("publish")]
    [ProducesResponseType(typeof(FacilityDetailPublishResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAcceptPublish(
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new FacilityGetGroupDetailsQuery(GroupOfFacility.Publish),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("reservation-change")]
    [ProducesResponseType(typeof(FacilityDetailReservationChangeResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReservationChange(
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new FacilityGetGroupDetailsQuery(GroupOfFacility.ReservationChange),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("reservation-setting")]
    [ProducesResponseType(typeof(FacilityDetailReservationSettingResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReservationSetting(
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new FacilityGetGroupDetailsQuery(GroupOfFacility.ReservationSetting),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("check-available")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public IActionResult CheckFacilityAvailable()
    {
        return Ok(true);
    }
}
