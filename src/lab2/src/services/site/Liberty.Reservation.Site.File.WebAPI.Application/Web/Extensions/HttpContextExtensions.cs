namespace Liberty.Reservation.Site.File.WebAPI.Application.Web.Extensions;

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
