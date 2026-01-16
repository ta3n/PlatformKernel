using PlatformKernel.Service.Application.Options;
using PlatformKernel.Service.WebApi.Application.Settings;
using PlatformKernel.ApplicationShared.Settings;
using PlatformKernel.Service.Base.Application.Settings;

namespace PlatformKernel.Service.WebApi.Configurations;

public static class AppSettingsStartup
{
    public static IServiceCollection AddAppSettingsModule(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.Configure<AppInfo>(configuration.GetSection("App"));
        services.Configure<ServiceSetting>(configuration.GetSection("Services"));
        services.Configure<SecretKeySetting>(configuration.GetSection("SecretKey"));
        services.Configure<ClientSetting>(configuration.GetSection("Client"));
        services.Configure<IntegrationEventSetting>(configuration.GetSection("IntegrationEvent"));
        services.Configure<ManagerModifyOptions>(configuration.GetSection("ManagerModify"));

        return services;
    }
}
