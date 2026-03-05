using Liberty.Reservation.Manager.File.WebAPI.Application;

namespace Liberty.Reservation.Manager.File.WebAPI.Configurations;

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
