using Liberty.Cache.Services;
using Liberty.Pagination;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful.Base;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.Destination;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.Destination;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.Facility;
using Liberty.Reservation.Employee.WebAPI.Application.Validations;
using Liberty.Reservation.Employee.WebAPI.Application.Web.Rest.Utilities;
using Liberty.SysException.Exceptions;
using MediatR;

namespace Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful;

[Route("api/destinations")]
public class DestinationsEndpoint(
    IMapper mapper,
    IMediator mediator,
    ICacheService cacheService,
    ISiteService siteService
) : BaseEndpoint(mapper, mediator)
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateDestination(
        [FromBody] DestinationCreateRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new DestinationCreateCommand { Payload = request },
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response.Id)
            .WithHeaders(
                HeaderUtil.CreateEntityCreationAlert(
                    nameof(Site),
                    response.Id.ToString()
                )
            );
    }

    [HttpPut("order")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public virtual async Task<IActionResult> ArrangeOrderOfDestinations(
        [FromBody] ItemUpdateOrderRequest request,
        CancellationToken cancellationToken
    )
    {
        var validation = await new ItemUpdateOrderRequestValidator().ValidateAsync(
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

        await siteService.ArrangeOrderAsync(
            request.Ids,
            cancellationToken
        );

        return NoContent();
    }

    [HttpPut("{id:long:min(1)}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateDestination(
        [FromRoute] long id,
        [FromBody] DestinationUpdateRequest request,
        CancellationToken cancellationToken
    )
    {
        request.Id = id;

        var response = await Mediator!.Send(
            new DestinationUpdateCommand { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(Site),
                    response.Id.ToString()
                )
            );
    }

    [HttpPatch("{id:long:min(1)}/enable")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> EnableDestination(
        [FromRoute] long id,
        [FromBody] DestinationEnabledRequest request,
        CancellationToken cancellationToken
    )
    {
        request.Id = id;

        var response = await siteService.EnableAsync(
            request.Id ?? 0,
            request.IsEnabled,
            cancellationToken: cancellationToken
        );

        await cacheService.RemoveByPatternsAsync(true, CacheKeys.ResetPatternManagerFacility);

        return ActionResultUtil.WrapOrNotFound(response.Id)
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(Site),
                    response.Id.ToString()
                )
            );
    }

    [HttpDelete("{id:long:min(1)}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> DeleteDestination(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var response = await siteService.DeleteAsync(id, cancellationToken: cancellationToken);
        await cacheService.RemoveByPatternsAsync(
            true,
            $"*{nameof(FacilityGetAllDestinationsQueryHandler)}*"
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityDeletionAlert(
                    nameof(Site),
                    response.Id.ToString()
                )
            );
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<DestinationResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllDestinations(
        IPageable pageable,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new DestinationGetAllQuery(pageable),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(DestinationDetailResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDestination(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new DestinationGetQuery(id),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }
}
