using Liberty.Reservation.Booking.Worker.Application;

namespace Liberty.Reservation.Booking.Worker.Configurations;

public static class AutoMapperStartup
{
    public static IServiceCollection AddAutoMapperModule(
        this IServiceCollection services
    )
    {
        services.AddAutoMapper(typeof(AssemblyDefinition));

        return services;
    }
}
