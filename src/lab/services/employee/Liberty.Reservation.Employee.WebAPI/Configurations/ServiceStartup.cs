using Liberty.Reservation.Employee.Application.Domains.Services;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;

namespace Liberty.Reservation.Employee.WebAPI.Configurations;

public static class ServiceStartup
{
    public static IServiceCollection AddServiceModule(
        this IServiceCollection services
    )
    {
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IAddressService, AddressService>();
        services.AddScoped<IEmployeeMetaService, EmployeeMetaService>();
        services.AddScoped<IFileService, FileService>();

        return services;
    }
}
