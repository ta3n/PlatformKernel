namespace Liberty.GmoPaymentGateway.Models.Requests;

public record PaymentGetUrlRequest(
    string? OrderId,
    int Amount,
    int Tax,
    string? LanguageCode
);
