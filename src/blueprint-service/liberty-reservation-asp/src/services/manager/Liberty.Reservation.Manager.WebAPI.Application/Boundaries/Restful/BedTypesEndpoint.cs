namespace Liberty.Reservation.Manager.WebAPI.Application.Boundaries.Restful;

[Route("api/bed-types")]
public class BedTypesEndpoint(
    IMapper mapper,
    IBedTypeService bedTypeService
) : BaseEndpoint(mapper)
{
    [HttpGet]
    [ProducesResponseType(typeof(List<BedTypeResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllBedTypes(
        IPageable pageable,
        CancellationToken cancellationToken
    )
    {
        var page = await bedTypeService.FindAllAsync(
            pageable,
            cancellationToken
        );
        var categories = page.Content;
        var response = Mapper.Map<List<BedTypeResponse>>(categories);
        var headers = page.GeneratePaginationHttpHeaders();

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }
}
