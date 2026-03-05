using Liberty.Pagination;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful.Base;
using Liberty.Reservation.Employee.WebAPI.Application.Validations;
using Liberty.Reservation.Employee.WebAPI.Application.Web.Rest.Utilities;
using Liberty.SysException.Exceptions;

namespace Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful;

[Route("api/app-date-types")]
public class AppDateTypesEndpoint(
    IMapper mapper,
    IAppDateTypeService appDateTypeService
) : BaseEndpoint(mapper)
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateAppDateType(
        [FromBody] AppDateTypeCreateRequest request,
        CancellationToken cancellationToken
    )
    {
        var validation = await new AppDateTypeCreateRequestValidator().ValidateAsync(
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

        var appDateType = Mapper.Map<AppDateType>(request);
        var response = await appDateTypeService.CreateAsync(appDateType, cancellationToken: cancellationToken);

        return ActionResultUtil.WrapOrNotFound(response.Id)
            .WithHeaders(
                HeaderUtil.CreateEntityCreationAlert(
                    nameof(AppDateType),
                    response.Id.ToString()
                )
            );
    }

    [HttpPut("order")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public virtual async Task<IActionResult> ArrangeOrderOfAppDateTypes(
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

        await appDateTypeService.ArrangeOrderAsync(
            request.Ids,
            cancellationToken
        );

        return NoContent();
    }

    [HttpPut("{id:long:min(1)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateAppDateType(
        [FromRoute] long id,
        [FromBody] AppDateTypeUpdateRequest request,
        CancellationToken cancellationToken
    )
    {
        request.Id = id;

        var validation = await new AppDateTypeUpdateRequestValidator().ValidateAsync(
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

        var appDateType = Mapper.Map<AppDateType>(request);
        var response = await appDateTypeService.UpdateAsync(appDateType, cancellationToken: cancellationToken);

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(AppDateType),
                    response.Id.ToString()
                )
            );
    }

    [HttpPatch("{id:long}/enable")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> EnableAppDateType(
        [FromRoute] long id,
        [FromBody] AppDateTypeEnabledRequest request,
        CancellationToken cancellationToken
    )
    {
        request.Id = id;

        var validation = await new AppDateTypeEnabledRequestValidator().ValidateAsync(
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

        var response = await appDateTypeService.EnableAsync(
            id,
            request.IsEnabled,
            false,
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(AppDateType),
                    response.Id.ToString()
                )
            );
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteAppDateType(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var response = await appDateTypeService.DeleteAsync(
            id,
            cancellationToken: cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityDeletionAlert(
                    nameof(AppDateType),
                    response.Id.ToString()
                )
            );
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<AppDateTypeResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAppDateTypes(
        IPageable pageable,
        CancellationToken cancellationToken
    )
    {
        var page = await appDateTypeService.FindAllAsync(pageable, cancellationToken);
        var appDateTypes = page.Content;
        var response = Mapper.Map<List<AppDateTypeResponse>>(appDateTypes);
        var headers = page.GeneratePaginationHttpHeaders();

        return Ok(response).WithHeaders(headers);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(AppDateTypeResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAppDateType(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var appDateType = await appDateTypeService.FindByIdAsync(id, cancellationToken);
        var response = Mapper.Map<AppDateTypeResponse>(appDateType);

        return ActionResultUtil.WrapOrNotFound(response);
    }
}
