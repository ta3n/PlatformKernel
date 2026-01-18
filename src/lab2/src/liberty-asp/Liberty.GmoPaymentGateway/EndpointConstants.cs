namespace Liberty.GmoPaymentGateway;

/// <summary>
/// Provides constant endpoint values used for GMO payment gateway API communication.
/// </summary>
public static class EndpointConstants
{
    /// <summary>
    /// Represents the endpoint for retrieving and searching trade details in the GMO Payment Gateway.
    /// This constant is used to build the URL for the "SearchTrade.idPass" API operation.
    /// </summary>
    public const string SearchTradeEndpoint = "SearchTrade.idPass";

    /// <summary>
    /// Represents the endpoint path used for the cancelation of online payment transactions.
    /// </summary>
    /// <remarks>
    /// This constant is used to build the URL for sending a cancel request to the GMO Payment Gateway system.
    /// </remarks>
    public const string CancelEndpoint = "AlterTran.idPass";

    /// <summary>
    /// Represents the constant endpoint string used to initiate the "Entry Transaction" process
    /// within the GMO Payment Gateway.
    /// </summary>
    /// <remarks>
    /// This endpoint, identified by the value "EntryTran.idPass", is used for preparing
    /// a transaction by registering it within the GMO payment gateway service. This is
    /// an essential step before executing a payment transaction.
    /// </remarks>
    public const string EntryTranEndpoint = "EntryTran.idPass";

    /// <summary>
    /// Represents the endpoint for executing payment transactions in the GMO Payment Gateway.
    /// This endpoint facilitates the transaction execution process by sending payment details
    /// such as access credentials, card information, and order ID.
    /// </summary>
    public const string ExecTranEndpoint = "ExecTran.idPass";

    /// <summary>
    /// Represents the endpoint used for changing the transaction amount
    /// in the GMO Payment Gateway system.
    /// </summary>
    /// <remarks>
    /// This constant is utilized to construct the request URL for operations
    /// related to modifying transaction amounts via the GMO Payment Gateway API.
    /// </remarks>
    public const string ChangeAmountEndpoint = "ChangeTran.idPass";
}

/// <summary>
/// Represents the result constants for GMO payment operations.
/// This class includes predefined constant strings to represent the status or
/// result of various GMO payment transactions such as success, error, refund outcomes, etc.
/// </summary>
/// <remarks>
/// The constants in this class are used throughout the GMO payment handling process.
/// They provide standardized values for identifying transaction results.
/// </remarks>
/// <example>
/// These constants can be useful for comparison or validation of
/// transaction statuses returned from GMO Payment Gateway operations.
/// </example>
public static class GmoPaymentResult
{
    /// <summary>
    /// Represents a constant value indicating a successful payment transaction in the GMO Payment Gateway.
    /// </summary>
    /// <remarks>
    /// This field is used to signify that a payment operation has been completed successfully.
    /// Typically utilized in scenarios where payment results are evaluated and handled, such as in
    /// reservation or order processing workflows.
    /// </remarks>
    public const string PaySuccess = "PAYSUCCESS";

    /// <summary>
    /// Represents the constant string value "ERROR" used to denote an error state in
    /// GMOPayment processes, such as payment failures or issues during transaction execution.
    /// </summary>
    /// <remarks>
    /// This constant is part of the GmoPaymentResult class and is commonly used to validate or
    /// compare payment operation outcomes, especially to identify error scenarios during its execution.
    /// </remarks>
    public const string Error = "ERROR";

    /// <summary>
    /// Represents the initial status of a payment process in the GMO Payment Gateway.
    /// This constant is used to signify that the payment process has started
    /// but has not yet completed or failed.
    /// </summary>
    public const string PayStart = "PAYSTART";

    /// <summary>
    /// Represents the "NotFound" status for the GMO payment result.
    /// This status is returned when a specified reservation or order is not found.
    /// </summary>
    public const string NotFound = "G001";

    /// <summary>
    /// Represents a constant value for invalid results in the GMO payment gateway process.
    /// </summary>
    /// <remarks>
    /// This value is used to indicate that the reservation or payment operation is in an invalid state.
    /// It is one of the predefined result codes returned by the GMO payment gateway.
    /// </remarks>
    public const string Invalid = "G002";

    /// <summary>
    /// Represents a success result code for a GMO payment transaction.
    /// When the payment process is successfully completed, this constant is used
    /// to indicate the successful operation in the payment gateway response or other related logic.
    /// </summary>
    public const string Success = "G004";

    /// <summary>
    /// Represents the constant result code "G005" which indicates a failed payment attempt in the GMO Payment Gateway integration.
    /// </summary>
    /// <remarks>
    /// This value is used in various logic flows to identify scenarios where payment processing encountered an error
    /// or was unsuccessful. It is a part of the predefined result codes of GMO Payment Gateway.
    /// </remarks>
    public const string Failed = "G005";

    /// <summary>
    /// Represents the result code indicating a successful refund operation.
    /// </summary>
    /// <remarks>
    /// The value of this constant is "G006". It is used to signify that a refund
    /// process was successful within the GMOPayment gateway context. This code is
    /// typically utilized for validation, logging, and error handling purposes.
    /// </remarks>
    public const string RefundSuccess = "G006";

    /// <summary>
    /// Represents the status code indicating that a refund operation has failed.
    /// </summary>
    /// <remarks>
    /// This constant is returned or referenced when an online payment refund attempt has not succeeded.
    /// It might be used in logging, error handling, or determining outcomes for refund-related processes.
    /// </remarks>
    public const string RefundFailed = "G007";

    /// <summary>
    /// Represents the specific error code indicating that a refund operation was successful
    /// but encountered a database-related error while processing the transaction.
    /// This constant is used in scenarios where a refund has succeeded in terms of business logic,
    /// but an auxiliary or logging operation involving the database failed.
    /// </summary>
    public const string RefundSuccessDbError = "G008";

    /// <summary>
    /// Represents the result code indicating a refund transaction failure
    /// due to a database error in the GMO Payment Gateway integration.
    /// </summary>
    /// <remarks>
    /// This constant is used to specify error scenarios where a refund operation
    /// could not be completed due to issues within the database, while interacting
    /// with the GMO payment platform.
    /// </remarks>
    public const string RefundFailDbError = "G009";

    /// <summary>
    /// Represents the error code "G0010" indicating a database error in the GMO payment process.
    /// </summary>
    public const string DbError = "G0010";
}
