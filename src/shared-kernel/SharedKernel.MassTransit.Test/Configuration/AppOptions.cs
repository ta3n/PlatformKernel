namespace SharedKernel.MassTransit.Test.Configuration;

public sealed record AppOptions
{
    public const string SectionName = "App";

    public string Role { get; init; } = AppRoles.Api;

    public static AppOptions FromConfiguration(
        IConfiguration configuration
    )
    {
        var options = configuration.GetSection(SectionName).Get<AppOptions>() ?? new AppOptions();
        return options with
        {
            Role = AppRoles.Normalize(options.Role)
        };
    }
}
