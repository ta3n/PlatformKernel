namespace Liberty.Fax.Options;

/// <summary>
/// Represents the configuration options for the fax service.
/// </summary>
public class FaxOptions
{
    /// <summary>
    /// Gets or sets the API URL used to send fax requests.
    /// </summary>
    /// <remarks>
    /// This property defines the endpoint for the fax service API. It is used when sending
    /// HTTP requests to the fax server. By default, it is set to "https://rest.faximo.jp/snd/v2/request.json".
    /// </remarks>
    public string? ApiUrl { get; set; } = "https://rest.faximo.jp/snd/v2/request.json";

    /// <summary>
    /// Gets or sets the user name for authentication with the Fax service API.
    /// </summary>
    /// <remarks>
    /// This property is used along with the <c>Password</c> property to create
    /// a Base64-encoded authentication header for requests sent to the Fax service.
    /// </remarks>
    public string? UserName { get; set; }

    /// <summary>
    /// Gets or sets the password used for authentication when sending faxes.
    /// This property, when combined with the <c>UserName</c>, is used to generate
    /// the authorization header required by the fax API.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Gets or sets the user authentication key used for accessing the Fax API service.
    /// This key is required for API interactions and ensures proper authorization.
    /// </summary>
    public string? UserKey { get; set; }

    /// <summary>
    /// Gets or sets the number of retry attempts for the fax operation.
    /// </summary>
    /// <remarks>
    /// This property specifies how many times the system should attempt to retry
    /// sending the fax in the event of a failure. The default value is 1.
    /// </remarks>
    public int RetryNum { get; set; } = 1;
}
