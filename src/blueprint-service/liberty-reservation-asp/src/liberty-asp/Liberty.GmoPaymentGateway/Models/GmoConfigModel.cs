namespace Liberty.GmoPaymentGateway.Models;

public record GmoConfigModel(
    string? PaymentHost,
    string? ShopId,
    string? ShopPassword
);
