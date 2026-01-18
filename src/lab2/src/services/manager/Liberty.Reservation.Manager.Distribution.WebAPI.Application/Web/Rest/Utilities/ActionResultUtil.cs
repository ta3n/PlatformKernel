using System.Collections;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Web.Rest.Utilities;

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

    public static ActionResult WrapOrNoContent(
        object response,
        object? dataToCheck
    )
    {
        if (dataToCheck == null || (dataToCheck is IEnumerable e && !e.Cast<object>().Any()))
        {
            return new NoContentResult();
        }

        return new OkObjectResult(response);
    }
}
