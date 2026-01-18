namespace Liberty.Reservation.Manager.File.WebAPI.Application.Web.Rest.Utilities;

public static class ActionResultUtil
{
    public static ActionResult WrapOrNotFoundAsDto<TDtoType>(
        object? value,
        IMapper mapper
    )
    {
        if (value is null)
        {
            return new NotFoundResult();
        }

        var resultAsDto = mapper.Map<TDtoType>(value);
        return new OkObjectResult(resultAsDto);
    }

    public static ActionResult WrapOrNotFound(
        object? value
    )
    {
        return value is not null ? new OkObjectResult(value) : new NotFoundResult();
    }

    public static ActionResult WrapFile(
        byte[] fileContent,
        string contentType
    )
    {
        return fileContent.Length > 0 ? new FileContentResult(fileContent, contentType) : new NotFoundResult();
    }
}
