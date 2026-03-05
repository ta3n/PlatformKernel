using Liberty.Reservation.Application.Domains.Services;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Site.Application.Domains.Services;
using Liberty.Reservation.Site.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Site.File.WebAPI.Configurations;

public static class ServiceStartup
{
    public static IServiceCollection AddServiceModule(
        this IServiceCollection services
    )
    {
        services.AddScoped(typeof(IBaseService<>), typeof(BaseService<>));
        services.AddScoped(typeof(IBaseServiceRelation<>), typeof(BaseServiceRelation<>));

        services.AddScoped<IMediaService, MediaService>();
        services.AddSingleton<ICacheManagementService, CacheManagementService>();

        return services;
    }
}
