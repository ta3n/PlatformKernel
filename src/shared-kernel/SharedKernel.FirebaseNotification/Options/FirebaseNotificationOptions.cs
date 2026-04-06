namespace SharedKernel.FirebaseNotification.Options;

public sealed class FirebaseNotificationOptions
{
    public const string SectionName = "FirebaseNotification";

    public string AppName { get; set; } = "shared-kernel-firebase-notification";

    public string? ProjectId { get; set; }

    public string? CredentialPath { get; set; }

    public string? CredentialJson { get; set; }

    public bool UseApplicationDefaultCredentials { get; set; } = true;

    public bool DefaultDryRun { get; set; }
}
