using Liberty.ApplicationShared.Settings;

namespace Liberty.Reservation.Manager.WebAPI.Application.Settings;

public class AppSetting : BaseAppSetting
{
    public DbConnectionStringSetting? DbConnectionStringSetting { get; set; }
    public WebCorsSetting? WebCorsSetting { get; set; }
}

public class ApiSetting
{
    public string? GroupNameFormat { get; set; }
}
