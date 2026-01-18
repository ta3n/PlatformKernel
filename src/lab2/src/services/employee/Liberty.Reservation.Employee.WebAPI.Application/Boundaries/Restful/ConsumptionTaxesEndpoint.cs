using Liberty.Pagination;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful.Base;
using Liberty.Reservation.Employee.WebAPI.Application.Validations;
using Liberty.Reservation.Employee.WebAPI.Application.Web.Rest.Utilities;
using Liberty.SysException.Exceptions;
using MediatR;
using Liberty.Reservation.Application.Utils;

namespace Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful;

[Route("api/consumption-taxes")]
public class ConsumptionTaxesEndpoint(
    IMapper mapper,
    IMediator mediator,
    IConsumptionTaxService consumptionTaxService
) : BaseEndpoint(mapper, mediator)
{
    [HttpPut("order")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public virtual async Task<IActionResult> ArrangeOrderOfConsumptionTaxes(
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

        await consumptionTaxService.ArrangeOrderAsync(
            request.Ids,
            cancellationToken
        );

        return NoContent();
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<ConsumptionTaxResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllConsumptionTaxes(
        IPageable pageable,
        CancellationToken cancellationToken
    )
    {
        var page = await consumptionTaxService.FindAllAsync(pageable, cancellationToken);
        var consumptionTaxes = page.Content;
        var response = Mapper.Map<List<ConsumptionTaxResponse>>(consumptionTaxes);
        var headers = page.GeneratePaginationHttpHeaders();

        return Ok(response).WithHeaders(headers);
    }

    [HttpGet("{id:long:min(1)}")]
    [ProducesResponseType(typeof(ConsumptionTaxResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetConsumptionTax(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var consumptionTax = await consumptionTaxService.FindByIdAsync(id, cancellationToken);
        var response = Mapper.Map<ConsumptionTaxResponse>(consumptionTax);

        return ActionResultUtil.WrapOrNotFound(response);
    }
}
