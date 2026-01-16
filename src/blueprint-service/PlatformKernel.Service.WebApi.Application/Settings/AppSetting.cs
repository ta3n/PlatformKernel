using PlatformKernel.ApplicationShared.Settings;

namespace PlatformKernel.Service.WebApi.Application.Settings;

public class AppSetting : BaseAppSetting
{
    public DbConnectionStringSetting? DbConnectionStringSetting { get; set; }
    public WebCorsSetting? WebCorsSetting { get; set; }
}

public class ApiSetting
{
    public string? GroupNameFormat { get; set; }
}
