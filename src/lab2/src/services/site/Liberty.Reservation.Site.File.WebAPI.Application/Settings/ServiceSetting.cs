namespace Liberty.Reservation.Site.File.WebAPI.Application.Settings;

public class ServiceSetting
{
    public ServiceConfig? MembershipFacilityService { get; set; }
}

public class ServiceConfig
{
    public string? ServiceName { get; set; }
    public string? Url { get; set; }
    public string? AccessCode { get; set; }
    public string? DataContextConnection { get; set; }
}
