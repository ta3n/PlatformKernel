using Liberty.ApplicationShared.Settings;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.File.WebAPI.Application.ExternalServices.Membership.Facility.Repositories;
using Liberty.ServiceDefaults.Middlewares;
using Liberty.SysException.Exceptions;
using Microsoft.Extensions.Options;

namespace Liberty.Reservation.Manager.File.WebAPI.Middlewares;

public class CheckFacilityAvailableMiddleware(
    IOptions<AppInfo> appInfoOptions,
    IFacilityExternalRepository facilityExternalRepository,
    IFacilityRepository facilityRepository
) : BaseMiddleware
{
    protected override async Task HandleAsync(
        HttpContext context,
        RequestDelegate next
    )
    {
        var isFacilityKeyExist = context.Request.Headers.TryGetValue(
            SecurityContextAccessor.FacilityHeaderKey,
            out var facilityCodeValue
        );
        var facilityCode = facilityCodeValue.ToString().Trim();
        var facilityKeyValid = !string.IsNullOrWhiteSpace(facilityCode);

        if (!isFacilityKeyExist || !facilityKeyValid)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync($"Error: '{SecurityContextAccessor.FacilityHeaderKey}' header is missing.");
            return;
        }

        var isFacilityAvailableOfMembership = await facilityExternalRepository.CheckFacilityAvailableAsync(facilityCode);
        var (isFacilityAvailableOfBooking, facilityId, _) = await facilityRepository.CheckFacilityAvailableAsync(facilityCode);
        var isFacilityAvailable = isFacilityAvailableOfMembership && isFacilityAvailableOfBooking;

        if (isFacilityAvailable)
        {
            context.Request.Headers.Append(
                SecurityContextAccessor.FacilityHeaderId,
                facilityId.ToString()
            );
            await next(context);

            return;
        }

        var appInfo = appInfoOptions.Value;
        var appException = new FacilityNotAvailableException();
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
