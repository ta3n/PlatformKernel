using Liberty.ApplicationShared.Settings;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.ServiceDefaults.Middlewares;
using Liberty.SysException.Exceptions;
using Microsoft.Extensions.Options;

namespace Liberty.Reservation.User.WebAPI.Middlewares;

public class CheckGuestCodeValidMiddleware(
    IOptions<AppInfo> appInfoOptions,
    IBookingSecureUrlService bookingSecureUrlService
) : BaseMiddleware
{
    protected override async Task HandleAsync(
        HttpContext context,
        RequestDelegate next
    )
    {
        if (context.Request.RouteValues.TryGetValue("code", out var codeValue))
        {
            var code = codeValue?.ToString();

            if (string.IsNullOrWhiteSpace(code))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync($"Invalid {code} parameter. Cannot be null or empty.");
                return;
            }

            var (isValid, _) = bookingSecureUrlService.DecryptAndValidate(code);

            if (isValid)
            {
                await next(context);
                return;
            }
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync("Missing required route parameter: code .");
            return;
        }

        var appInfo = appInfoOptions.Value;
        var appException = new GuestReservationCodeIncorrectException();
        var exceptionObject = new
        {
            // Exception
            code = appException.ErrorCode.ToString(),
            title = appException.Title,
            description = appException.Message,
            app = new
            {
                name = appInfo.AppName,
                version = appInfo.AppVersion,
                date = $"{DateTime.UtcNow}"
            }
        };

        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(exceptionObject);
    }
}
