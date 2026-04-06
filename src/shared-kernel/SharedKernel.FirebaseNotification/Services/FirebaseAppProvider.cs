using System.Text.Json;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedKernel.FirebaseNotification.Options;

namespace SharedKernel.FirebaseNotification.Services;

internal sealed class FirebaseAppProvider(
    IOptions<FirebaseNotificationOptions> options,
    ILogger<FirebaseAppProvider> logger
) : IDisposable
{
    private readonly object _syncRoot = new();
    private FirebaseApp? _firebaseApp;

    public FirebaseApp GetApp()
    {
        if (_firebaseApp is not null)
        {
            return _firebaseApp;
        }

        lock (_syncRoot)
        {
            if (_firebaseApp is not null)
            {
                return _firebaseApp;
            }

            var options1 = options.Value;
            var appName = ResolveAppName(options1);
            _firebaseApp = FirebaseApp.GetInstance(appName)
                ?? FirebaseApp.Create(CreateAppOptions(options1), appName);

            logger.LogInformation(
                "Firebase app {AppName} initialized for project {ProjectId}.",
                _firebaseApp.Name,
                GetProjectId()
            );

            return _firebaseApp;
        }
    }

    public string? GetProjectId()
    {
        var options1 = options.Value;

        if (!string.IsNullOrWhiteSpace(options1.ProjectId))
        {
            return options1.ProjectId.Trim();
        }

        if (!string.IsNullOrWhiteSpace(options1.CredentialJson))
        {
            return TryReadProjectId(options1.CredentialJson);
        }

        if (!string.IsNullOrWhiteSpace(options1.CredentialPath))
        {
            var credentialPath = ResolveCredentialPath(options1.CredentialPath);
            if (File.Exists(credentialPath))
            {
                return TryReadProjectId(File.ReadAllText(credentialPath));
            }
        }

        return FirstNonEmpty(
            Environment.GetEnvironmentVariable("GOOGLE_CLOUD_PROJECT"),
            Environment.GetEnvironmentVariable("GCLOUD_PROJECT")
        );
    }

    public void Dispose()
    {
        lock (_syncRoot)
        {
            if (_firebaseApp is null)
            {
                return;
            }

            try
            {
                _firebaseApp.Delete();
            }
            catch (InvalidOperationException exception)
            {
                logger.LogWarning(
                    exception,
                    "Firebase app {AppName} was already deleted.",
                    _firebaseApp.Name
                );
            }
            finally
            {
                _firebaseApp = null;
            }
        }
    }

    private static AppOptions CreateAppOptions(
        FirebaseNotificationOptions options
    )
    {
        return new AppOptions
        {
            Credential = ResolveCredential(options),
            ProjectId = string.IsNullOrWhiteSpace(options.ProjectId) ? null : options.ProjectId.Trim()
        };
    }

    private static GoogleCredential ResolveCredential(
        FirebaseNotificationOptions options
    )
    {
        if (!string.IsNullOrWhiteSpace(options.CredentialJson))
        {
            return CredentialFactory.FromJson(
                options.CredentialJson,
                JsonCredentialParameters.ServiceAccountCredentialType
            );
        }

        if (!string.IsNullOrWhiteSpace(options.CredentialPath))
        {
            var credentialPath = ResolveCredentialPath(options.CredentialPath);

            if (!File.Exists(credentialPath))
            {
                throw new InvalidOperationException(
                    $"Firebase credential file was not found at '{credentialPath}'."
                );
            }

            return CredentialFactory.FromFile(
                credentialPath,
                JsonCredentialParameters.ServiceAccountCredentialType
            );
        }

        if (options.UseApplicationDefaultCredentials)
        {
            return GoogleCredential.GetApplicationDefault();
        }

        throw new InvalidOperationException(
            "Firebase credential is not configured. Provide CredentialPath, CredentialJson, or enable application default credentials."
        );
    }

    private static string ResolveAppName(
        FirebaseNotificationOptions options
    )
    {
        return string.IsNullOrWhiteSpace(options.AppName)
            ? "shared-kernel-firebase-notification"
            : options.AppName.Trim();
    }

    private static string ResolveCredentialPath(
        string credentialPath
    )
    {
        var trimmedPath = credentialPath.Trim();
        return Path.IsPathRooted(trimmedPath)
            ? trimmedPath
            : Path.GetFullPath(trimmedPath, Directory.GetCurrentDirectory());
    }

    private static string? TryReadProjectId(
        string json
    )
    {
        try
        {
            using var jsonDocument = JsonDocument.Parse(json);
            if (!jsonDocument.RootElement.TryGetProperty("project_id", out var projectIdElement))
            {
                return null;
            }

            return projectIdElement.ValueKind == JsonValueKind.String
                ? projectIdElement.GetString()
                : null;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static string? FirstNonEmpty(
        params string?[] values
    )
    {
        return values
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value!.Trim())
            .FirstOrDefault();
    }
}
