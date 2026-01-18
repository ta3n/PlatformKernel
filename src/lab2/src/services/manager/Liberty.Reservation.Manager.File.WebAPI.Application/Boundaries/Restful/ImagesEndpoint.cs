using Liberty.Reservation.Manager.File.WebAPI.Application.UserCases.Commands.Image;
using Liberty.Reservation.Manager.File.WebAPI.Application.UserCases.Queries.Image;
using Liberty.Reservation.Manager.File.WebAPI.Application.Web.Extensions;
using Liberty.Reservation.Manager.File.WebAPI.Application.Web.Rest.Utilities;

namespace Liberty.Reservation.Manager.File.WebAPI.Application.Boundaries.Restful;

[Route("api/images")]
public class ImagesEndpoint(
    IMapper mapper,
    IMediator mediator
) : BaseEndpoint(mapper, mediator)
{
    [HttpGet]
    [ProducesResponseType(typeof(List<ImageResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllImages(
        IPageable pageable,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new ImageGetAllQuery(
                pageable
            ),
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
    }

    [HttpGet("{code}/{sizeType?}")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetImagePreview(
        [FromRoute] string code,
        [FromRoute] string? sizeType,
        CancellationToken cancellationToken
    )
    {
        var (headers, response) = await Mediator!.Send(
            new ImageGetQuery(code, sizeType),
            cancellationToken
        );

        if (response.Image is null)
        {
            return ActionResultUtil.WrapOrNotFound(response).WithHeaders(headers);
        }

        if (response.Image.CanSeek)
        {
            response.Image.Position = 0;
        }

        return new FileStreamResult(
                response.Image,
                response.ContentType
            ) { EnableRangeProcessing = true }
            .WithHeaders(headers);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateImage(
        [FromForm] ImageCreateRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new ImageCreateCommand { Payload = request },
            cancellationToken
        );

        return ActionResultUtil.WrapOrNotFound(response)
            .WithHeaders(
                HeaderUtil.CreateEntityCreationAlert(
                    nameof(Reservation.Application.Contexts.DataContexts.Entities.Data.File),
                    response.ToString()
                )
            );
    }

    [HttpPut("{id:min(1)}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateImage(
        [FromRoute] long id,
        [FromBody] ImageUpdateRequest request,
        CancellationToken cancellationToken
    )
    {
        request.Id = id;

        var response = await Mediator!.Send(
            new ImageUpdateCommand { Payload = request },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(Reservation.Application.Contexts.DataContexts.Entities.Data.File),
                    response.ToString()
                )
            );
    }

    [HttpDelete("{id:long:min(1)}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteImage(
        [FromRoute] long id,
        CancellationToken cancellationToken
    )
    {
        var response = await Mediator!.Send(
            new ImageDeleteCommand { Payload = new ImageDeleteRequest(id) },
            cancellationToken
        );

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityDeletionAlert(
                    nameof(Reservation.Application.Contexts.DataContexts.Entities.Data.File),
                    response.ToString()
                )
            );
    }
}
