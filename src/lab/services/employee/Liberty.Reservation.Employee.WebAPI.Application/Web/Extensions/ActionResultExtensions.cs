using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Liberty.Reservation.Employee.WebAPI.Application.Web.Extensions;

public static class ActionResultExtensions
{
    public static ActionResult WithHeaders(
        this ActionResult receiver,
        IHeaderDictionary headers
    )
    {
        return new ActionResultWithHeaders(receiver, headers);
    }
}
