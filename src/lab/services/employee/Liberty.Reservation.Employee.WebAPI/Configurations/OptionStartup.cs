namespace Liberty.Reservation.Employee.WebAPI.Configurations;

public static class OptionStartup
{
    public static IServiceCollection AddCustomOptions(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        return services;
    }
}
