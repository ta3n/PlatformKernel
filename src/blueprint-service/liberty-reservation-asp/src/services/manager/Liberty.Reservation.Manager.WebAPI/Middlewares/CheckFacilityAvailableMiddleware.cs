using Liberty.ApplicationShared.Settings;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.ExternalServices.Membership.Facility.Repositories;
using Liberty.Reservation.Manager.WebAPI.Application.ExternalServices.Membership.Facility.Services;
using Liberty.ServiceDefaults.Middlewares;
using Liberty.SysException.Exceptions;
using Microsoft.Extensions.Options;

namespace Liberty.Reservation.Manager.WebAPI.Middlewares;

public class CheckFacilityAvailableMiddleware(
    IOptions<AppInfo> appInfoOptions,
    IFacilityExternalRepository facilityExternalRepository,
    IFacilityRepository facilityRepository,
    ICheckFacilityService checkFacilityService
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
        var facilityKeyValid = string.IsNullOrEmpty(facilityCode);

        if (!isFacilityKeyExist || facilityKeyValid)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync($"Error: '{SecurityContextAccessor.FacilityHeaderKey}' header is missing.");
            return;
        }

        var isFacilityAvailableOfMembership = await facilityExternalRepository.CheckFacilityAvailableAsync(facilityCode);

        if (isFacilityAvailableOfMembership)
        {
            var managerClaim = context.User.Claims.FirstOrDefault(c => c.Type == "code");
            var managerId = "";
            if (managerClaim != null)
            {
                managerId = managerClaim.Value;
            }

            var facilityOfMembership = await checkFacilityService.CheckUserManagedFacilityAsync(
                facilityCode,
                managerId
            );
            var isUserHaveFacilityPermission = facilityOfMembership is not null;
            if (!isUserHaveFacilityPermission)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            var (isFacilityAvailable, facilityId) = await facilityRepository.CheckFacilityAvailableAsync(facilityCode);
            if (isFacilityAvailable)
            {
                context.Request.Headers.Append(
                    SecurityContextAccessor.FacilityHeaderId,
                    $"{facilityId}"
                );
                context.Request.Headers.Append(
                    SecurityContextAccessor.FacilityHeaderRecordCode,
                    facilityOfMembership!.Meta.FirstOrDefault(x => x.Key == "code")?.Value
                );

                await next(context);
                return;
            }
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
