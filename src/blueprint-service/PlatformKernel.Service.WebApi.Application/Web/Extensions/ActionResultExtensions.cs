namespace PlatformKernel.Service.WebApi.Application.Web.Extensions;

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
