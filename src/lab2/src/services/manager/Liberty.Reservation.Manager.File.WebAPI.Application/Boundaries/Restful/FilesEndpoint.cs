using Liberty.ApplicationShared.Utils;
using Liberty.Reservation.Manager.File.WebAPI.Application.Web.Extensions;
using Liberty.Reservation.Manager.File.WebAPI.Application.Web.Rest.Utilities;
using Microsoft.AspNetCore.Authorization;

namespace Liberty.Reservation.Manager.File.WebAPI.Application.Boundaries.Restful;

[AllowAnonymous]
[ApiExplorerSettings(GroupName = "v2")]
[Route("api/files")]
public class FilesEndpoint(
    IMapper mapper
) : BaseEndpoint(mapper)
{
    /// <summary>
    /// Retrieves all files based on the provided query parameters.
    /// </summary>
    /// <param name="request">The request object containing query parameters for filtering files.</param>
    /// <param name="pageable">The pageable object for pagination information.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>A list of files wrapped in an IActionResult with a status code of 200 OK.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<FileResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAllFiles(
        [FromQuery] FileGetAllRequest request,
        IPageable pageable,
        CancellationToken cancellationToken
    )
    {
        var headers = new HeaderDictionary();
        return ActionResultUtil.WrapOrNotFound(null).WithHeaders(headers);
    }

    /// <summary>
    /// Retrieves a file based on the provided code.
    /// </summary>
    /// <param name="code">The code of the file to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>A file wrapped in an IActionResult with a status code of 200 OK if found, otherwise 404 Not Found.</returns>
    [HttpGet("{code}")]
    [ProducesResponseType(typeof(FileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetFile(
        [FromRoute] string code,
        CancellationToken cancellationToken
    )
    {
        var headers = new HeaderDictionary();
        return ActionResultUtil.WrapOrNotFound(null).WithHeaders(headers);
    }

    /// <summary>
    /// Retrieves a file preview based on the provided code and size type.
    /// </summary>
    /// <param name="code">The code of the file to retrieve the preview for.</param>
    /// <param name="sizeType">The size type of the file preview to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>A file preview wrapped in an IActionResult with a status code of 200 OK if found, otherwise 404 Not Found.</returns>
    [HttpGet("{code}/view/{sizeType}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult GetFilePreview(
        [FromRoute] string code,
        [FromRoute] string sizeType,
        CancellationToken cancellationToken
    )
    {
        var headers = new HeaderDictionary();
        return File(new MemoryStream(), string.Empty)
            .WithHeaders(headers);
    }

    /// <summary>
    /// Creates a new file based on the provided request data.
    /// </summary>
    /// <param name="request">The request object containing data for the new file.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>A newly created file wrapped in an IActionResult with a status code of 200 OK.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(FileResponse), StatusCodes.Status200OK)]
    public IActionResult CreateFile(
        [FromForm] FileCreateRequest request,
        CancellationToken cancellationToken
    )
    {
        var newFileCode = $"{Guid.NewGuid():N}";
        var newFile = new FileResponse(
            1,
            newFileCode,
            [],
            string.Empty,
            true,
            1,
            ConvertUtil.ToLong(
                ConvertUtil.ToString(DateTime.UtcNow, "yyyyMMddHHmmssfff")
            ),
            request.FilePurposeType ?? FilePurposeTypes.Unknown,
            [],
            []
        );

        return ActionResultUtil.WrapOrNotFound(newFile)
            .WithHeaders(
                HeaderUtil.CreateEntityCreationAlert(
                    nameof(Reservation.Application.Contexts.DataContexts.Entities.Data.File),
                    newFileCode
                )
            );
    }

    /// <summary>
    /// Updates an existing file based on the provided code and request data.
    /// </summary>
    /// <param name="code">The code of the file to update.</param>
    /// <param name="request">The request object containing updated data for the file.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>An IActionResult with a status code of 200 OK.</returns>
    [HttpPut("{code}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult UpdateFile(
        [FromRoute] string code,
        [FromForm] FileUpdateRequest request,
        CancellationToken cancellationToken
    )
    {
        request.Code = code;

        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityUpdateAlert(
                    nameof(Reservation.Application.Contexts.DataContexts.Entities.Data.File),
                    code
                )
            );
    }

    /// <summary>
    /// Deletes a file based on the provided code.
    /// </summary>
    /// <param name="code">The code of the file to delete.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>An IActionResult with a status code of 200 OK.</returns>
    [HttpDelete("{code}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult DeleteFile(
        [FromRoute] string code,
        CancellationToken cancellationToken
    )
    {
        return NoContent()
            .WithHeaders(
                HeaderUtil.CreateEntityDeletionAlert(
                    nameof(Reservation.Application.Contexts.DataContexts.Entities.Data.File),
                    code
                )
            );
    }

    /// <summary>
    /// Retrieves all link files associated with the provided record code.
    /// </summary>
    /// <param name="recordCode">The record code to retrieve the link files for.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>A list of link files wrapped in an IActionResult with a status code of 200 OK.</returns>
    [HttpGet("{recordCode}/links")]
    [ProducesResponseType(typeof(List<FileLinkResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAllLinkFiles(
        [FromRoute] string recordCode,
        CancellationToken cancellationToken
    )
    {
        return NoContent();
    }

    /// <summary>
    /// Adjusts link files associated with the provided record code.
    /// </summary>
    /// <param name="recordCode">The record code to adjust the link files for.</param>
    /// <param name="request">The request object containing data for adjusting the link files.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>An IActionResult with a status code of 204 No Content.</returns>
    [HttpPut("{recordCode}/links")]
    public IActionResult AdjustLinkFiles(
        [FromRoute] string recordCode,
        [FromBody] FileLinksUpdateRequest request,
        CancellationToken cancellationToken
    )
    {
        request.RecordCode = recordCode;

        return NoContent();
    }
}
