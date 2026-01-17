using Blueprint.Service.WebApi.Application;

namespace PlatformKernel.Service.WebApi.Configurations;

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
