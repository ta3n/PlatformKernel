using System.Text.Json.Serialization;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models;

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
}
