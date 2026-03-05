namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Settings;

public class WebCorsSetting
{
    public string? PolicyName { get; set; }
    public string? AllowedOrigins { get; set; } = "*";
    public string? AllowedMethods { get; set; } = "*";
    public string? AllowedHeaders { get; set; } = "*";
    public string? ExposedHeaders { get; set; } = "*";
    public bool AllowCredentials { get; set; }
    public int MaxAge { get; set; }
    public bool EnforceHttps { get; set; }

    public string[] GetAllowedOrigins()
    {
        return AllowedOrigins?.Split(',') ?? ["*"];
    }

    public string[] GetAllowedMethods()
    {
        return AllowedMethods?.Split(',') ?? ["*"];
    }

    public string[] GetAllowedHeaders()
    {
        return AllowedHeaders?.Split(',') ?? ["*"];
    }

    public string[] GetExposedHeaders()
    {
        return ExposedHeaders?.Split(',') ?? ["*"];
    }
}
