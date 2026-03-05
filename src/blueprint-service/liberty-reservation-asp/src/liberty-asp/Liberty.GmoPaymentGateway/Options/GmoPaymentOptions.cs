namespace Liberty.GmoPaymentGateway.Options;

/// <summary>
/// Represents the configuration options for GMO Payment Gateway.
/// </summary>
public record GmoPaymentOptions
{
    /// <summary>
    /// Determines whether the GMO payment gateway integration is enabled.
    /// If set to true, the integration will be active, allowing interactions with the GMO payment system.
    /// If false, the integration will be disabled, and related services will not be configured or utilized.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets the URL endpoint for the payment gateway link.
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// Gets or sets the host URL used for payment processing in the GMO Payment Gateway.
    /// </summary>
    /// <remarks>
    /// This property specifies the base URL required to communicate with the GMO Payment Gateway for
    /// executing payment-related operations such as transactions, inquiries, and validations.
    /// </remarks>
    public string? PaymentHost { get; set; }

    /// Represents the Shop ID required to integrate with the GMO Payment Gateway.
    /// This identifier is used to authenticate and associate payment requests
    /// with the appropriate shop in the payment system.
    public string? ShopId { get; set; }

    /// <summary>
    /// Represents the password of the shop for GMO payment gateway authentication.
    /// </summary>
    /// <remarks>
    /// This property is used as part of the GMO payment API requests to authenticate the shop.
    /// Ensure the value is kept confidential and secure, as it is a sensitive credential.
    /// </remarks>
    public string? ShopPassword { get; set; }

    /// <summary>
    /// Specifies the job code for the payment transaction.
    /// This property defines the type of payment operation to perform, such as "CAPTURE"
    /// to immediately charge the payment or other supported operations based on GmoPayment setup.
    /// </summary>
    public string? JobCd { get; set; }

    /// <summary>
    /// Gets or sets the 3D Secure 2.0 type for the payment transaction.
    /// The value determines the authentication method used for 3D Secure 2.0.
    /// Default is 3, which typically indicates normal authorization.
    /// </summary>
    public int Tds2Type { get; set; } = 3; // Default to normal authorization

    /// <summary>
    /// Gets or sets the flag indicating whether 3D Secure (TDS) is enabled for the payment transaction.
    /// </summary>
    public int TdFlag { get; set; } = 0;

    /// <summary>
    /// Specifies the format for an order date used in the GmoPaymentOptions configuration.
    /// The format string should follow the standard .NET date and time format conventions.
    /// </summary>
    public string? OrderDateFormat { get; set; }

    /// Represents a configuration property to determine whether the credit payment option is used in GMO Payment Gateway integration.
    /// The value is typically represented as an integer, where different values may indicate availability or non-availability
    /// of credit payment support. For example, the value might be `1` for enabled or `0` for disabled.
    /// This property is utilized when configuring GMO payment options within the application.
    public int UseCredit { get; set; }

    /// <summary>
    /// Gets or sets the expiration time (in minutes) for the payment process.
    /// </summary>
    public int Expire { get; set; }

    /// <summary>
    /// Specifies the endpoint URL for obtaining the payment URL.
    /// This property is used in API calls to interact with the GMO Payment Gateway
    /// and retrieve a link for payment processing.
    /// </summary>
    public string? UrlPayment { get; set; }

    /// <summary>
    /// Gets or sets the configuration identifier used for the payment gateway settings.
    /// This value is typically required to associate the request with a specific configuration in the GMO payment system.
    /// </summary>
    public string? ConfigId { get; set; }

    /// <summary>
    /// Gets or sets the URL to which the payment gateway redirects after processing the payment.
    /// This is used for handling the result of the payment transaction on the client side.
    /// </summary>
    public string? RetUrl { get; set; }

    /// <summary>
    /// Gets or sets the URL that will be called upon the successful completion of a payment process.
    /// This property is typically used to notify the system or redirect the user to a confirmation page
    /// after the payment transaction has been successfully completed.
    /// </summary>
    public string? CompleteUrl { get; set; }

    /// The URL that will be used to handle cancellations during the payment process.
    /// This property is typically utilized to redirect users to a specific endpoint
    /// when they choose to cancel the transaction.
    public string? CancelUrl { get; set; }

    /// <summary>
    /// A property representing the payment methods available for transactions.
    /// The value is expected to be a comma-separated string of payment methods.
    /// </summary>
    public string? PayMethods { get; set; }

    /// Splits the PayMethods property into an array of payment method strings.
    /// If PayMethods is null or empty, defaults to returning an array containing "credit".
    /// <returns>An array of payment method strings.</returns>
    public string[] GetPayMethods()
    {
        return PayMethods?.Split(",") ?? ["credit"];
    }
}
