namespace Liberty.Reservation.Booking.Worker.Application.Settings;

public class ServiceSetting
{
    public ServiceConfig? BatchSchedulerService { get; set; }
    public ServiceConfig? ReservationSiteService { get; set; }
}

public class ServiceConfig
{
    public string? ServiceName { get; set; }
    public string? Url { get; set; }
    public string? AccessCode { get; set; }
    public string? DataContextConnection { get; set; }
}
