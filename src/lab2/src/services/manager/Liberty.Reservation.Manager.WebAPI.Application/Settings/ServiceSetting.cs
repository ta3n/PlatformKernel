namespace Liberty.Reservation.Manager.WebAPI.Application.Settings;

public class ServiceSetting
{
    public ServiceConfig? ReservationEmployeeService { get; set; }
    public ServiceConfig? MembershipFacilityService { get; set; }
    public ServiceConfig? BatchSchedulerService { get; set; }
}

public class ServiceConfig
{
    public string? ServiceName { get; set; }
    public string? Url { get; set; }
    public string? AccessCode { get; set; }
    public string? DataContextConnection { get; set; }
}
