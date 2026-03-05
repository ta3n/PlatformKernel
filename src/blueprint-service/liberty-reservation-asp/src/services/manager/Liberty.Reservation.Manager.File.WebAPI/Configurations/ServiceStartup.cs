using Liberty.Reservation.Application.Domains.Services;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.Application.Domains.Services;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.File.WebAPI.Application.Web.ApiService;

namespace Liberty.Reservation.Manager.File.WebAPI.Configurations;

public static class ServiceStartup
{
    public static IServiceCollection AddServiceModule(
        this IServiceCollection services
    )
    {
        services.AddScoped(typeof(IBaseService<>), typeof(BaseService<>));
        services.AddScoped(typeof(IBaseServiceRelation<>), typeof(BaseServiceRelation<>));

        services.AddScoped<IExternalApiService, ExternalApiService>();

        services.AddSingleton<ICacheManagementService, CacheManagementService>();

        services.AddScoped<IFilePlanService, FilePlanService>();
        services.AddScoped<IFileRoomGroupService, FileRoomGroupService>();
        services.AddScoped<IFileOptionItemService, FileOptionItemService>();
        services.AddScoped<IFacilityFileService, FacilityFileService>();

        services.AddScoped<IFileCategoryService, FileCategoryService>();
        services.AddScoped<ICategoryService, CategoryService>();

        return services;
    }
}
