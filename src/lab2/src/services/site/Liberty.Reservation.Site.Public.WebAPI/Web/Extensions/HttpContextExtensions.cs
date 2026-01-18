namespace Liberty.Reservation.Site.Public.WebAPI.Web.Extensions;

public static class HttpContextExtensions
{
    public static string GetClientIp(
        this HttpContext context
    )
    {
        return context.Request.Headers["X-Forwarded-For"].FirstOrDefault()
            ?? context.Connection.RemoteIpAddress?.ToString()
            ?? "unknown";
    }
}
