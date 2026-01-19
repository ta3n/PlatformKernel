using Microsoft.Extensions.DependencyInjection;

namespace BlueprintCqrs.Configuration;

public static class AutoMapperStartup
{
    public static IServiceCollection AddAutoMapperModule(
        this IServiceCollection services
    )
    {
        services.AddAutoMapper(typeof(Startup));
        return services;
    }
}
