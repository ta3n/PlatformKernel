using Liberty.Reservation.Employee.WebAPI.Application;

namespace Liberty.Reservation.Employee.WebAPI.Configurations;

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
