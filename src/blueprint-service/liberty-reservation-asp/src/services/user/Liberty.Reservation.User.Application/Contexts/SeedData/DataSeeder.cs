namespace Liberty.Reservation.User.Application.Contexts.SeedData;

public static class DataSeeder
{
    public static Task SeedingAsync(
        string webRootPath,
        UserDataContext context
    )
    {
        return Task.CompletedTask;
    }
}
