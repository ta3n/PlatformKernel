namespace Liberty.GmoPaymentGateway;

/// <summary>
/// Provides a set of constants representing parameter keys used in communication with the GMO Payment Gateway.
/// </summary>
public static class ParameterConstants
{
    /// <summary>
    /// Represents the constant key for the Shop ID parameter used in various requests
    /// to the GMO Payment Gateway.
    /// </summary>
    /// <remarks>
    /// This parameter is required to identify the merchant's shop within the GMO Payment system.
    /// It is typically included in the data sent to the gateway during payment-related operations.
    /// </remarks>
    public const string ShopId = "ShopID";

    /// <summary>
    /// Represents the constant parameter name used for specifying the shop password in GMO Payment Gateway API requests.
    /// </summary>
    /// <remarks>
    /// This is a predefined constant value that corresponds to the string "ShopPass", which is used as a key in dictionary-based API request data.
    /// It is primarily used in operations such as payment trade searches and cancellations within the GMO Payment Gateway service.
    /// </remarks>
    public const string ShopPass = "ShopPass";

    /// <summary>
    /// Represents the identifier of an order used to uniquely identify payment transactions
    /// in the GMO Payment Gateway service.
    /// </summary>
    public const string OrderId = "OrderID";

    /// <summary>
    /// A constant string representing the parameter key for "AccessID" used in communication with the GMO Payment Gateway API.
    /// This parameter is required to authenticate and identify a specific transaction or session.
    /// </summary>
    public const string AccessId = "AccessID";

    /// <summary>
    /// Represents the constant key name "AccessPass" used for API parameterization in
    /// GMO Payment Gateway requests.
    /// </summary>
    /// <remarks>
    /// This value is utilized in constructing request payloads for various endpoints,
    /// such as payment cancellations and execution transactions. It typically corresponds
    /// to the access pass credential required for API authentication.
    /// </remarks>
    public const string AccessPass = "AccessPass";

    /// <summary>
    /// Represents the constant parameter "JobCd" used in GMO Payment Gateway API requests to specify the type of transaction.
    /// Common values include "VOID" for transaction cancellations and "AUTH" for payment authorizations.
    /// </summary>
    public const string JobCd = "JobCd";

    /// <summary>
    /// Represents the transaction amount used in payment operations.
    /// </summary>
    /// <remarks>
    /// The value of this constant typically denotes the monetary amount associated with
    /// a transaction in the GMO Payment Gateway system. It is utilized as a parameter
    /// for various API requests, such as payment registration and transaction cancellation.
    /// </remarks>
    public const string Amount = "Amount";

    /// <summary>
    /// Represents the constant key for specifying tax or consumption tax in payment-related requests.
    /// </summary>
    /// <remarks>
    /// This field is used as a parameter key in multiple requests handled by the GMO Payment Gateway,
    /// such as in transaction entry (EntryTranAsync) or cancellation (CancelAsync) processes.
    /// Corresponds to the tax amount applicable to the transaction.
    /// </remarks>
    public const string Tax = "Tax";

    /// <summary>
    /// Represents the payment method identifier constant used to indicate the method of payment in
    /// transactions (e.g., credit card, convenience store, etc.).
    /// </summary>
    /// <remarks>
    /// This constant is primarily used in the GMOPayment requests to specify the type of payment
    /// method being processed. Typical values include identifiers for payment methods supported
    /// by the payment gateway.
    /// </remarks>
    /// <value>
    /// A string representing the payment method.
    /// </value>
    public const string Method = "Method";

    /// <summary>
    /// Represents the card number used for payment transactions in the GMO Payment Gateway system.
    /// </summary>
    public const string CardNo = "CardNo";

    /// <summary>
    /// Represents the expiration date of the credit card in 'YYMM' format.
    /// This parameter is used in payment transactions to specify the card's expiry date.
    /// </summary>
    public const string Expire = "Expire";

    /// <summary>
    /// Represents the security code (CCV) of a credit card used in transactions.
    /// </summary>
    public const string SecurityCode = "SecurityCode";
}
