using Liberty.Reservation.User.File.WebAPI.Application;

namespace Liberty.Reservation.User.File.WebAPI.Configurations;

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
