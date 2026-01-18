namespace Liberty.Reservation.Site.Public.WebAPI.Settings;

public class AppSetting : BaseAppSetting
{
    public WebCorsSetting? WebCorsSetting { get; set; }
}

public class IdentitySetting
{
    public string? Authority { get; set; }
    public IdentityAccessCodeConfig? AccessCode { get; set; }
}

public class IdentityAccessCodeConfig
{
    public string? Code { get; set; }
}

public class ApiSetting
{
    public string? GroupNameFormat { get; set; }
}

public class AcquireLock
{
    public int? ExpirationTime { get; set; }
}
