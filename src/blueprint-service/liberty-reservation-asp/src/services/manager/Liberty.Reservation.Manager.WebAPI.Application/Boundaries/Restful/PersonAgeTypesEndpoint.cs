namespace Liberty.Reservation.Manager.WebAPI.Application.Boundaries.Restful;

[Route("api/person-age-types")]
public class PersonAgeTypesEndpoint(
    IMapper mapper,
    IMediator mediator,
    IPersonAgeTypeService personAgeTypeService
) : BaseEndpoint(mapper, mediator)
{
    [HttpGet]
    [ProducesResponseType(typeof(List<PersonAgeTypeResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllPersonAgeTypes(
        IPageable pageable,
        CancellationToken cancellationToken
    )
    {
        var page = await personAgeTypeService.GetAllPersonAgeTypes(pageable, true, cancellationToken);

        var data = page.Content;
        var response = Mapper.Map<List<PersonAgeTypeResponse>>(data);
        var headers = page.GeneratePaginationHttpHeaders();

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }
}
