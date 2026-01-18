using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.PriceType;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.PriceType;

namespace Liberty.Reservation.Manager.WebAPI.Application.Boundaries.Restful;

[Route("api/price-types")]
public class PriceTypesEndpoint(
    IMapper mapper,
    IMediator mediator,
    IAppDateTypeService appDateTypeService,
    ICacheManagementService cacheManagementService
) : BaseEndpoint(mapper, mediator)
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreatePriceType(
        [FromBody] PriceTypeCreateRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new PriceTypeCreateCommand { Payload = request },
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response)
            .WithHeaders(
                HeaderUtil.CreateEntityCreationAlert(
                    "PriceType",
                    response.ToString()
                )
            );
    }

    [HttpPut("order")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public virtual async Task<IActionResult> ArrangeOrderOfPriceTypes(
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

        await appDateTypeService.ArrangeOrderAsync(
            request.Ids,
            cancellationToken
        );

        cacheManagementService.RemoveFacilityRelatedCache();

        return NoContent();
    }

    [HttpPut("{id:long:min(1)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdatePriceType(
        [FromRoute] long id,
        [FromBody] PriceTypeUpdateRequest request,
        CancellationToken cancellationToken
    )
    {
        request.Id = id;

        var response = await Mediator!.Send(
            new PriceTypeUpdateCommand { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    "PriceType",
                    response.ToString()
                )
            );
    }

    [HttpDelete("{id:long:min(1)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeletePriceType(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new PriceTypeDeleteCommand { Payload = new PriceTypeDeleteRequest { Id = id } },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityDeletionAlert(
                    "PriceType",
                    response.ToString()
                )
            );
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<PriceTypeResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllPriceTypes(
        IPageable pageable,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new PriceTypeGetAllQuery(pageable),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("{id:long:min(1)}")]
    [ProducesResponseType(typeof(PriceTypeResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPriceType(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var (header, response) = await Mediator!.Send(
            new PriceTypeGetQuery(id),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(header);
    }
}
