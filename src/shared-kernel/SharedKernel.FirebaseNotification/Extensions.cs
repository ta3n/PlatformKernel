using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.FirebaseNotification.Abstractions;
using SharedKernel.FirebaseNotification.Options;
using SharedKernel.FirebaseNotification.Services;

namespace SharedKernel.FirebaseNotification;

public static class Extensions
{
    public static IServiceCollection AddFirebaseNotification(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = FirebaseNotificationOptions.SectionName
    )
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentException.ThrowIfNullOrWhiteSpace(sectionName);

        services.AddOptions<FirebaseNotificationOptions>()
            .Bind(configuration.GetSection(sectionName))
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.AppName),
                $"{sectionName}:AppName is required."
            )
            .Validate(
                options => !(HasCredentialPath(options) && HasCredentialJson(options)),
                $"{sectionName} supports only one credential source between CredentialPath and CredentialJson."
            )
            .Validate(
                options => HasCredentialPath(options) || HasCredentialJson(options) || options.UseApplicationDefaultCredentials,
                $"{sectionName} requires CredentialPath, CredentialJson or UseApplicationDefaultCredentials=true."
            )
            .ValidateOnStart();

        services.AddSingleton<FirebaseAppProvider>();
        services.AddSingleton<IFirebaseNotificationService, FirebaseNotificationService>();

        return services;
    }

    private static bool HasCredentialPath(
        FirebaseNotificationOptions options
    )
    {
        return !string.IsNullOrWhiteSpace(options.CredentialPath);
    }

    private static bool HasCredentialJson(
        FirebaseNotificationOptions options
    )
    {
        return !string.IsNullOrWhiteSpace(options.CredentialJson);
    }
}
