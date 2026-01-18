using Liberty.Fax.Options;
using Liberty.Fax.Services;
using Liberty.Fax.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Liberty.Fax;

/// <summary>
/// Provides extension methods for configuring fax-related services.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Configures and registers the FaxImo options and services with the provided service collection.
    /// </summary>
    /// <param name="services">The service collection to which the FaxImo options and services will be added.</param>
    /// <param name="configuration">The application configuration containing the FaxImo settings.</param>
    /// <returns>The updated service collection with the FaxImo configuration and services registered.</returns>
    public static IServiceCollection AddFaxImo(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var section = configuration.GetSection("FaxImo");
        var faxOptions = section.Get<FaxOptions>();
        services.Configure<FaxOptions>(section);

        if (faxOptions is null)
        {
            return services;
        }

        services.AddScoped<IFaxService, FaxService>();

        return services;
    }
}
