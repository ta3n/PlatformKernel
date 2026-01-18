namespace Liberty.Reservation.Site.Public.WebAPI.Settings;

public class ServiceSetting
{
    public ServiceConfig? SiteService { get; set; }
}

public class ServiceConfig
{
    public string? ServiceName { get; set; }
    public string? Url { get; set; }
    public string? AccessCode { get; set; }
    public string? DataContextConnection { get; set; }
}
