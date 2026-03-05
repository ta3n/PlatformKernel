namespace Liberty.Reservation.Manager.WebAPI.Application.Boundaries.Restful;

[Route("api/meal-types")]
public class MealTypesEndpoint(
    IMapper mapper,
    IMealTypeService mealTypeService
) : BaseEndpoint(mapper)
{
    [HttpGet]
    [ProducesResponseType(typeof(List<MealTypeResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllMealTypes(
        IPageable pageable,
        CancellationToken cancellationToken
    )
    {
        var page = await mealTypeService.FindAllAsync(
            pageable,
            cancellationToken
        );
        var mealTypes = page.Content;
        var response = Mapper.Map<List<MealTypeResponse>>(mealTypes);
        var headers = page.GeneratePaginationHttpHeaders();

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }
}
