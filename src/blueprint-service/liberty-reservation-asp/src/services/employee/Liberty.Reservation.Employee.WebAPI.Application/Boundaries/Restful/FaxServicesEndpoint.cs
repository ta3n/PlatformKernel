using Liberty.Pagination;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful.Base;
using Liberty.Reservation.Employee.WebAPI.Application.Web.Rest.Utilities;
using MediatR;

namespace Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful;

[Route("api/fax-service-categories")]
public class FaxServicesEndpoint(
    IMapper mapper,
    IMediator mediator,
    IFaxSrvService faxSrvService
) : BaseEndpoint(mapper, mediator)
{
    [HttpGet]
    [ProducesResponseType(typeof(List<FaxServiceResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllFaxServices(
        IPageable pageable,
        CancellationToken cancellationToken
    )
    {
        var page = await faxSrvService.FindAllAsync(
            pageable,
            cancellationToken
        );
        var categories = page.Content;
        var response = Mapper.Map<List<FaxServiceResponse>>(categories);
        var headers = page.GeneratePaginationHttpHeaders();

        return Ok(response).WithHeaders(headers);
    }
}
