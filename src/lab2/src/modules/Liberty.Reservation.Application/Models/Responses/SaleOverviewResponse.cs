namespace Liberty.Reservation.Application.Models.Responses;

public record SaleOverviewResponse(
    int TotalCountOfOnSidePayments,
    int TotalCountOfOnlinePayments,
    int TotalCountOfModifiedPayments,
    int TotalCountOfCancelledPayments,
    decimal TotalLocalPaymentPrice,
    decimal TotalOnlinePaymentPrice,
    decimal TotalPaymentPrice,
    decimal CardFee,
    decimal BankTransferFee,
    decimal ExpectedDepositAmount
)
{
    public int TotalCountOfReservations => TotalCountOfOnSidePayments
        + TotalCountOfOnlinePayments
        + TotalCountOfModifiedPayments
        + TotalCountOfCancelledPayments;
}
