using Liberty.Reservation.Site.File.WebAPI.Application.Boundaries.Restful.Base;
using Liberty.Reservation.Site.File.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Site.File.WebAPI.Application.UserCases.Queries.Media;
using Liberty.Reservation.Site.File.WebAPI.Application.Web.Extensions;
using Liberty.Reservation.Site.File.WebAPI.Application.Web.Rest.Utilities;
using Microsoft.AspNetCore.Authorization;

namespace Liberty.Reservation.Site.File.WebAPI.Application.Boundaries.Restful;

[AllowAnonymous]
[Route("api/images")]
public class ImagesEndpoint(
    IMapper mapper,
    IMediator mediator
) : BaseEndpoint(mapper, mediator)
{
    [HttpGet("{code}/{sizeType?}")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> PreviewImage(
        [FromRoute] string code,
        [FromRoute] string? sizeType,
        CancellationToken cancellationToken
    )
    {
        var (headers, (contentType, image)) = await Mediator!.Send(
            new ImageGetQuery(
                new ImagePreviewRequest(code, sizeType)
            ),
            cancellationToken
        );

        return image is null
            ? ActionResultUtil.WrapOrNotFound(image).WithHeaders(headers)
            : File(image, contentType).WithHeaders(headers);
    }
}
