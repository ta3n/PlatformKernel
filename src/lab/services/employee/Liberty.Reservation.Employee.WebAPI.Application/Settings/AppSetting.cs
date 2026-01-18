using Liberty.ApplicationShared.Settings;

namespace Liberty.Reservation.Employee.WebAPI.Application.Settings;

public class AppSetting : BaseAppSetting
{
    public DbConnectionStringSetting? DbConnectionStringSetting { get; set; }
    public WebCorsSetting? WebCorsSetting { get; set; }
}
