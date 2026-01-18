using System.Text.Json;
using System.Text.Json.Serialization;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.WebAPI.Application.ExternalServices.Membership.Facility.Dtos;
using Liberty.Reservation.Manager.WebAPI.Application.Settings;
using Liberty.Reservation.Manager.WebAPI.Application.Web.ApiService;
using Microsoft.Extensions.Options;

namespace Liberty.Reservation.Manager.WebAPI.Application.ExternalServices.Membership.Facility.Services.Implements;

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

    public async Task<ManagerFacilityDto?> CheckUserManagedFacilityAsync(
        string facilityCode,
        string managerId,
        CancellationToken cancellationToken = default
    )
    {
        var facilityResponse = await GetAllFacilitiesFromMembershipAsync(
            managerId,
            cancellationToken
        );

        if (facilityResponse is null || facilityResponse.Facilities is { Count: <= 0 })
        {
            return null;
        }

        return facilityResponse.Facilities.Find(x => x.Code == facilityCode);
    }

    public async Task<ManagerFacilityDtoResponse?> GetAllFacilitiesFromMembershipAsync(
        string managerId,
        CancellationToken cancellationToken = default
    )
    {
        var url = $"api/External/manager/facility/many?managerId={managerId}&api-version=1";

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
