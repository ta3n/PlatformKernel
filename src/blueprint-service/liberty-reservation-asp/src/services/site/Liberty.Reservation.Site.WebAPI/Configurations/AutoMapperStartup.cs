using Liberty.Reservation.Site.WebAPI.Application;

namespace Liberty.Reservation.Site.WebAPI.Configurations;

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
