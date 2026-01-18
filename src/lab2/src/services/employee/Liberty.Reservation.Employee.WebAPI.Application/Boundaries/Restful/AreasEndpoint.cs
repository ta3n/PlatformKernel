using Liberty.Pagination;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful.Base;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.Area;
using Liberty.Reservation.Employee.WebAPI.Application.Validations;
using Liberty.Reservation.Employee.WebAPI.Application.Web.Rest.Utilities;
using Liberty.SysException.Exceptions;
using MediatR;

namespace Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful;

[Route("api/areas")]
public class AreasEndpoint(
    IMapper mapper,
    IMediator mediator,
    IAreaService areaService
) : BaseEndpoint(mapper)
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateArea(
        [FromBody] AreaCreateRequest request,
        CancellationToken cancellationToken
    )
    {
        var validation = await new AreaCreateRequestValidator().ValidateAsync(
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

        var area = Mapper.Map<Area>(request);
        var response = await areaService.CreateAsync(area, cancellationToken: cancellationToken);

        return ActionResultUtil.WrapOrNotFound(response.Id)
            .WithHeaders(
                HeaderUtil.CreateEntityCreationAlert(
                    nameof(Area),
                    response.Id.ToString()
                )
            );
    }

    [HttpPut("order")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public virtual async Task<IActionResult> ArrangeOrderOfAreas(
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

        await areaService.ArrangeOrderAsync(
            request.Ids,
            cancellationToken
        );

        return NoContent();
    }

    [HttpPut("{id:long:min(1)}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateArea(
        [FromRoute] long id,
        [FromBody] AreaUpdateRequest request,
        CancellationToken cancellationToken
    )
    {
        request.Id = id;

        var validation = await new AreaUpdateRequestValidator().ValidateAsync(
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

        var area = Mapper.Map<Area>(request);
        var response = await areaService.UpdateAsync(area, cancellationToken: cancellationToken);

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(Area),
                    response.Id.ToString()
                )
            );
    }

    [HttpPatch("{id:long:min(1)}/enable")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> EnableArea(
        [FromRoute] long id,
        [FromBody] AreaEnabledRequest request,
        CancellationToken cancellationToken
    )
    {
        request.Id = id;

        var response = await mediator.Send(
            new AreaEnableCommand { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(Area),
                    response.ToString()
                )
            );
    }

    [HttpDelete("{id:long:min(1)}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> DeleteArea(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var response = await areaService.DeleteAsync(
            id,
            cancellationToken: cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityDeletionAlert(
                    nameof(Area),
                    response.Id.ToString()
                )
            );
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<AreaResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAreas(
        IPageable pageable,
        CancellationToken cancellationToken
    )
    {
        var page = await areaService.FindAllAsync(pageable, cancellationToken);
        var areas = page.Content;
        var response = Mapper.Map<List<AreaResponse>>(areas);
        var headers = page.GeneratePaginationHttpHeaders();

        return Ok(response).WithHeaders(headers);
    }

    [HttpGet("{id:long:min(1)}")]
    [ProducesResponseType(typeof(AreaResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetArea(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var area = await areaService.FindByIdAsync(id, cancellationToken);
        var response = Mapper.Map<AreaResponse>(area);

        return ActionResultUtil.WrapOrNotFound(response);
    }
}
