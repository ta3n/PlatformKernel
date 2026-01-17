using Blueprint.Service.Application.Options;
using Blueprint.Service.Base.Application.Settings;
using Blueprint.Service.WebApi.Application.Settings;
using SharedKernel.ApplicationShared.Settings;

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
