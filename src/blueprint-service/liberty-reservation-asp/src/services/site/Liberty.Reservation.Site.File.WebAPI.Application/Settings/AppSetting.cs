using Liberty.ApplicationShared.Settings;

namespace Liberty.Reservation.Site.File.WebAPI.Application.Settings;

public class AppSetting : BaseAppSetting
{
    public DbConnectionStringSetting? DbConnectionStringSetting { get; set; }
    public WebCorsSetting? WebCorsSetting { get; set; }
}

public class IdentitySetting
{
    public string? Authority { get; set; }
}

public class ApiSetting
{
    public string? GroupNameFormat { get; set; }
}

public class AcquireLock
{
    public int? ExpirationTime { get; set; }
}
