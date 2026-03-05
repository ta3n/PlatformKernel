using Liberty.ApplicationShared.Domains.Services.Mails;
using Liberty.Reservation.Application.Domains.Services;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Employee.Application.Domains.Services;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Employee.WebAPI.Configurations;

public static class ServiceStartup
{
    public static IServiceCollection AddServiceModule(
        this IServiceCollection services
    )
    {
        services.AddScoped(typeof(IBaseService<>), typeof(BaseService<>));
        services.AddScoped(typeof(IBaseServiceRelation<>), typeof(BaseServiceRelation<>));

        services.AddScoped<ILanguageService, LanguageService>();

        services.AddScoped<IMailService, SmtpMailService>();
        services.AddScoped<ISystemConfigService, SystemConfigService>();
        services.AddScoped<IAppDateTypeService, AppDateTypeService>();
        services.AddScoped<IFacilityService, FacilityService>();
        services.AddScoped<IFacilityCategoryService, FacilityCategoryService>();
        services.AddScoped<IFacilitySiteService, FacilitySiteService>();
        services.AddScoped<ISiteService, SiteService>();
        services.AddScoped<IAreaService, AreaService>();
        services.AddScoped<IConsumptionTaxService, ConsumptionTaxService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IMailTypeService, MailTypeService>();
        services.AddScoped<IFaxSrvService, FaxSrvService>();
        services.AddScoped<IFacilityFaxSrvService, FacilityFaxSrvService>();
        services.AddScoped<IAppDateService, AppDateService>();
        services.AddScoped<IAppDateDataService, AppDateDataService>();
        services.AddScoped<IAppDateTypeService, AppDateTypeService>();
        services.AddScoped<IDateDataOfAppDateService, DateDataOfAppDateService>();
        services.AddScoped<IDateTypeOfAppDateService, DateTypeOfAppDateService>();
        services.AddScoped<IFacilityInitDefaultDataService, FacilityInitDefaultDataService>();
        services.AddScoped<IFacilityInitStatusService, FacilityInitStatusService>();
        services.AddScoped<IBookingReservationService, BookingReservationService>();
        services.AddScoped<IAlertMessageService, AlertMessageService>();
        services.AddScoped<IBookingCancellationService, BookingCancellationService>();

        return services;
    }
}
