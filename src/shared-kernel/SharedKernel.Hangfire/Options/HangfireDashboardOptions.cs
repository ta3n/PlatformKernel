namespace SharedKernel.Hangfire.Options;

/// <summary>
/// Represents configuration options for the Hangfire dashboard integration.
/// </summary>
public sealed record HangfireDashboardOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HangfireDashboardOptions"/> class.
    /// </summary>
    public HangfireDashboardOptions()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HangfireDashboardOptions"/> class.
    /// </summary>
    /// <param name="enabled">A value indicating whether the dashboard is enabled.</param>
    /// <param name="dashboardUrl">The dashboard path.</param>
    /// <param name="username">The dashboard username.</param>
    /// <param name="password">The dashboard password.</param>
    /// <param name="isReadOnly">A value indicating whether the dashboard is read-only.</param>
    public HangfireDashboardOptions(
        bool enabled,
        string? dashboardUrl,
        string? username,
        string? password,
        bool isReadOnly
    )
    {
        Enabled = enabled;
        DashboardUrl = dashboardUrl;
        Username = username;
        Password = password;
        IsReadOnly = isReadOnly;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the dashboard is enabled.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets the dashboard path.
    /// </summary>
    public string? DashboardUrl { get; set; }

    /// <summary>
    /// Gets or sets the dashboard username.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Gets or sets the dashboard password.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the dashboard is read-only.
    /// </summary>
    public bool IsReadOnly { get; set; }
}
