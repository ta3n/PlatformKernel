using System.Collections.Generic;
using Newtonsoft.Json;

namespace BlueprintCqrs.Dto.ProfileInfo;

public class ProfileInfoDto(
    string displayRibbonOnProfiles,
    List<string> activeProfiles
)
{
    [JsonProperty("display-ribbon-on-profiles")]
    public string DisplayRibbonOnProfiles { get; set; } = displayRibbonOnProfiles;

    [JsonProperty("activeProfiles")]
    public List<string> ActiveProfiles { get; set; } = activeProfiles;
}
