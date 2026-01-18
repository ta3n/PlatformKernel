using Liberty.Reservation.User.WebAPI.Application;

namespace Liberty.Reservation.User.WebAPI.Configurations;

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
