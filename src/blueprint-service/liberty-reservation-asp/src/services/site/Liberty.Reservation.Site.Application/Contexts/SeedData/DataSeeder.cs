namespace Liberty.Reservation.Site.Application.Contexts.SeedData;

public static class DataSeeder
{
    public static Task SeedingAsync(
        string webRootPath,
        SiteDataContext context
    )
    {
        return Task.CompletedTask;
    }
}
