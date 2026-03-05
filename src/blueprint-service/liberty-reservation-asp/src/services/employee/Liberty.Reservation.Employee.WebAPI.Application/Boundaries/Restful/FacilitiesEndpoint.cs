using Liberty.Pagination;
using Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful.Base;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.Facility;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.Facility;
using Liberty.Reservation.Employee.WebAPI.Application.Web.Rest.Utilities;
using MediatR;

namespace Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful;

[Route("api/facilities")]
[ApiVersion(1)]
public class FacilitiesEndpoint(
    IMapper mapper,
    IMediator mediator
) : BaseEndpoint(mapper, mediator)
{
    [HttpPut("{id:long:min(1)}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateFacility(
        [FromRoute] long id,
        [FromBody] FacilityUpdateRequest request,
        CancellationToken cancellationToken
    )
    {
        request.Id = id;

        var response = await Mediator!.Send(
            new FacilityUpdateCommand { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(Facility),
                    response.Id.ToString()
                )
            );
    }

    [HttpPatch("{id:long:min(1)}/fax-service")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateFaxOfFacility(
        [FromRoute] long id,
        [FromBody] FacilityUpdateFaxServiceRequest request,
        CancellationToken cancellationToken
    )
    {
        request.Id = id;

        var response = await Mediator!.Send(
            new FacilityUpdateFaxServiceCommand { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(Facility),
                    response.Id.ToString()
                )
            );
    }

    [HttpGet("{id:long:min(1)}/fax-service")]
    [ProducesResponseType(typeof(FacilityDetailFaxResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFaxOfFacility(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new FacilityGetFaxQuery(id),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpPatch("{id:long:min(1)}/enable")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> EnableFacility(
        [FromRoute] long id,
        [FromBody] FacilityEnabledRequest request,
        CancellationToken cancellationToken
    )
    {
        request.Id = id;

        var response = await Mediator!.Send(
            new FacilityEnableCommand { Payload = request },
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

    [HttpPatch("{id:long:min(1)}/seed-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SeedDataFacility(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new FacilityInitSeedDataCommand { Payload = new FacilityInitSeedDataRequest(id) },
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

    [HttpGet]
    [ProducesResponseType(typeof(List<FacilityResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllFacilities(
        IPageable pageable,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new FacilityGetAllQuery(pageable),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("{id:long:min(1)}")]
    [ProducesResponseType(typeof(FacilityDetailResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFacility(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var (header, response) = await Mediator!.Send(
            new FacilityGetQuery(id),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(header);
    }

    [HttpGet("{id:long:min(1)}/seed-status")]
    [ProducesResponseType(typeof(FacilitySeedStatusResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInitStatusOfFacility(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var (header, response) = await Mediator!.Send(
            new FacilityGetSeedStatusQuery(id),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(header);
    }

    [HttpGet("{id:long:min(1)}/destinations")]
    [ProducesResponseType(typeof(DestinationResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllDestinationsOfFacility(
        [FromRoute] long id,
        IPageable pageable,
        CancellationToken cancellationToken
    )
    {
        var (header, response) = await Mediator!.Send(
            new FacilityGetAllDestinationsQuery(pageable) { FacilityId = id },
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(header);
    }

    [HttpPost("{id:long:min(1)}/send-fax")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SendFaxOfFacility(
        [FromRoute] long id,
        [FromBody] FacilitySendFaxRequest request,
        CancellationToken cancellationToken
    )
    {
        request.Id = id;

        var (_, response) = await Mediator!.Send(
            new FacilitySendFaxCommand { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateAlert(
                    string.Empty,
                    response.Result ?? string.Empty
                )
            );
    }
}
