using System.Text.Json.Serialization;

namespace Liberty.Reservation.Manager.WebAPI.Application.ExternalServices.Membership.Facility.Dtos;

public class ManagerFacilityDtoResponse
{
    public List<ManagerFacilityDto> Facilities { get; set; } = [];
}

public class ManagerFacilityDto
{
    [JsonPropertyName("Id")]
    public string? Code { get; set; }

    public string? Name { get; set; }

    public bool IsEnabled { get; set; } = true;

    public IEnumerable<ManagerFacilityMetaDto> Meta { get; set; } = [];
}

public class ManagerFacilityMetaDto
{
    public string? Key { get; set; }
    public string? Value { get; set; }
}
