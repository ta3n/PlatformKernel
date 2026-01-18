using Liberty.Pagination;
using Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful.Base;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.PersonAgeType;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.PersonAgeType;
using Liberty.Reservation.Employee.WebAPI.Application.Web.Rest.Utilities;
using MediatR;

namespace Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful;

[Route("api/person-age-type")]
public class PersonAgeTypeEndpoint(
    IMapper mapper,
    IMediator mediator
) : BaseEndpoint(mapper, mediator)
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PersonAgeTypeResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetListPersonAgeType(
        IPageable pageable,
        CancellationToken cancellationToken = default
    )
    {
        var (header, response) = await Mediator!.Send(
            new PersonAgeTypeGetAllQuery(pageable),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(header);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreatePersonAgeType(
        [FromBody] PersonAgeTypeCreateRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new PersonAgeTypeCreateCommand { Payload = request },
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response)
            .WithHeaders(
                HeaderUtil.CreateEntityCreationAlert(
                    nameof(PersonAgeType),
                    response.ToString()
                )
            );
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdatePersonAgeType(
        [FromBody] PersonAgeTypeUpdateRequest request,
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        request.Id = id;
        var response = await Mediator!.Send(
            new PersonAgeTypeUpdateCommand { Payload = request },
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response)
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(PersonAgeType),
                    response.ToString()
                )
            );
    }

    [HttpDelete("{id:long:min(1)}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> DeletePersonAgeType(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new PersonAgeTypeDeleteCommand { Payload = new PersonAgeTypeDeleteRequest(id) },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityDeletionAlert(
                    nameof(PersonAgeType),
                    response.ToString()
                )
            );
    }
}
