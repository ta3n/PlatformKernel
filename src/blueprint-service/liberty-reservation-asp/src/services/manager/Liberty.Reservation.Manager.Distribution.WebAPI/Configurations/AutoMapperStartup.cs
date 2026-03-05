using Liberty.Reservation.Manager.Distribution.WebAPI.Application;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Configurations;

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
