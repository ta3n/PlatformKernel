using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful.Base;
using Liberty.Reservation.Employee.WebAPI.Application.Web.Rest.Utilities;
using MediatR;

namespace Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful;

[Route("api/mail-types")]
public class MailTypesEndpoint(
    IMapper mapper,
    IMediator mediator,
    IMailTypeService mailTypeService
) : BaseEndpoint(mapper, mediator)
{
    [HttpGet]
    [ProducesResponseType(typeof(List<MailTypeResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAllMailTypes()
    {
        var mailTypes = mailTypeService.FindAll();
        var response = mailTypes.Select(
            x => new MailTypeResponse(x.IoType, x.Name)
        );

        return ActionResultUtil.WrapOrNotFound(response);
    }
}
