using Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful.Base;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.MailTemplate;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.MailTemplate;
using Liberty.Reservation.Employee.WebAPI.Application.Web.Rest.Utilities;
using MediatR;

namespace Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful;

[Route("api/mail-templates")]
public class MailTemplatesEndpoint(
    IMapper mapper,
    IMediator mediator
) : BaseEndpoint(mapper, mediator)
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> SendMail(
        [FromBody] MailTemplateSendRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new MailTemplateSendCommand(request),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response);
    }

    [HttpPut("{type}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateMailTemplate(
        [FromRoute] string type,
        [FromBody] MailTemplateUpdateRequest request,
        CancellationToken cancellationToken
    )
    {
        request.IoType = type;

        var response = await Mediator!.Send(
            new MailTemplateUpdateCommand { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    "MailTemplate",
                    $"{response}"
                )
            );
    }

    [HttpGet("{type}")]
    [ProducesResponseType(typeof(MailTemplateResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMailTemplate(
        [FromRoute] string type,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new MailTemplateGetQuery(type),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpPost("{type}/preview")]
    [ProducesResponseType(typeof(MailTemplatePreviewResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> PreviewMail(
        [FromBody] MailTemplatePreviewRequest request,
        [FromRoute] string type,
        CancellationToken cancellationToken
    )
    {
        request.IoType = type;
        var response = await Mediator!.Send(
            new MailTemplatePreviewCommand(request),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response);
    }
}
