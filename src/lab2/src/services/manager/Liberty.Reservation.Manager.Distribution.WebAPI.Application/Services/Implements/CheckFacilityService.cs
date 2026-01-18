using System.Text.Json;
using System.Text.Json.Serialization;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Settings;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Web.ApiService;
using Microsoft.Extensions.Options;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Services.Implements;

public class CheckFacilityService(
    IExternalApiService externalApiService,
    ISecurityContextAccessor securityContextAccessor,
    IOptions<ServiceSetting> serviceSettingOption
) : ICheckFacilityService
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReferenceHandler = ReferenceHandler.Preserve
    };

    public async Task<bool> CheckUserManagedFacilityAsync(
        string facilityCode,
        string userCode,
        CancellationToken cancellationToken = default
    )
    {
        var facilityResponse = await GetAllFacilitiesFromMembershipAsync(
            userCode,
            cancellationToken
        );

        if (facilityResponse is null || facilityResponse.Facilities is { Count: <= 0 })
        {
            return false;
        }

        return facilityResponse.Facilities.Exists(x => x.Code == facilityCode && x.IsEnabled);
    }

    public async Task<bool> CheckUserManagedFacilityAsync(
        string[] facilityCodes,
        string userCode,
        CancellationToken cancellationToken = default
    )
    {
        var facilityResponse = await GetAllFacilitiesFromMembershipAsync(
            userCode,
            cancellationToken
        );

        if (facilityResponse is null || facilityResponse.Facilities is { Count: <= 0 })
        {
            return false;
        }

        var availableFacilityCodes = facilityResponse.Facilities.Where(x => x.IsEnabled).Select(x => x.Code).ToArray();

        return Array.TrueForAll(facilityCodes, x => availableFacilityCodes.Contains(x));
    }

    public async Task<ManagerFacilityDtoResponse?> GetAllFacilitiesFromMembershipAsync(
        string userCode,
        CancellationToken cancellationToken = default
    )
    {
        var url = $"api/External/manager/facility/many?managerId={userCode}&api-version=1";

        var (_, data) = await externalApiService.GetAsync(
            ExternalService.MembershipService,
            url,
            new Dictionary<string, string>
            {
                { "access_code", serviceSettingOption.Value.MembershipFacilityService!.AccessCode! },
                { "authorization", securityContextAccessor.JwtTokenRaw ?? string.Empty }
            },
            cancellationToken
        );

        if (string.IsNullOrEmpty(data))
        {
            return new ManagerFacilityDtoResponse();
        }

        var facilityResponse = JsonSerializer.Deserialize<ManagerFacilityDtoResponse>(
            data,
            _jsonOptions
        );

        return facilityResponse;
    }
}
