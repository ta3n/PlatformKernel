using Liberty.Reservation.Manager.WebAPI.Application;

namespace Liberty.Reservation.Manager.WebAPI.Configurations;

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
