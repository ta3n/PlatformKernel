namespace Liberty.GmoPaymentGateway.Models.Requests;

public record PaymentExecTranRequest(
    string? OrderId,
    string? AccessId,
    string? AccessPass,
    string? CardNumber,
    string? CCV,
    string? ExpiryDate
);
