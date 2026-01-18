using Liberty.Reservation.Site.File.WebAPI.Application;

namespace Liberty.Reservation.Site.File.WebAPI.Configurations;

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
