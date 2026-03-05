namespace Liberty.Reservation.User.File.WebAPI.Application.Settings;

public class DbConnectionStringSetting
{
    public string? InMemoryDatabase { get; set; }

    public string? MigrationsAssembly { get; set; }

    public string? DataContextConnection { get; set; }
}
