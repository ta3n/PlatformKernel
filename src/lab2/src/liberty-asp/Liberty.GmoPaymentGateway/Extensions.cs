using Liberty.GmoPaymentGateway.Options;
using Liberty.GmoPaymentGateway.Services;
using Liberty.GmoPaymentGateway.Services.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Liberty.GmoPaymentGateway;

/// <summary>
/// Provides extension methods for configuring GMO Payment Gateway support
/// in an application.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Configures and adds the GMO Payment Gateway services to the dependency injection container
    /// based on the provided application configuration. If the GMO payment gateway is disabled in
    /// configuration, the service will not be added.
    /// </summary>
    /// <param name="services">The service collection to which the GMO Payment Gateway service will be added.</param>
    /// <param name="configuration">The application configuration that contains the GMO Payment Gateway settings.</param>
    /// <returns>The updated service collection with the GMO Payment Gateway service added if enabled.</returns>
    public static IServiceCollection AddGmoPaymentGateway(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var section = configuration.GetSection("GMOPayment");
        var gmoPaymentOptions = section.Get<GmoPaymentOptions>();

        if (gmoPaymentOptions is null || !gmoPaymentOptions.Enabled)
        {
            return services;
        }

        services.Configure<GmoPaymentOptions>(section);
        services.AddScoped<IGmoPaymentGatewayService, GmoPaymentGatewayService>();

        return services;
    }

    public static IServiceCollection AddGmoErrorService(
        this IServiceCollection services
    )
    {
        services.AddSingleton<IGmoErrorCodeService, GmoErrorCodeService>();

        return services;
    }
}
