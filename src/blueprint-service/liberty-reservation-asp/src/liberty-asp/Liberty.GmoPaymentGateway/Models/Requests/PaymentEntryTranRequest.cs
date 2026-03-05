namespace Liberty.GmoPaymentGateway.Models.Requests;

public record PaymentEntryTranRequest(
    string? OrderId,
    string? CardNumber,
    string? Cvv,
    decimal Amount,
    decimal Tax,
    string? Currency
);
