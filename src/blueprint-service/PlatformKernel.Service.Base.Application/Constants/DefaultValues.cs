namespace PlatformKernel.Service.Base.Application.Constants;

public static class DefaultValues
{
    public const int TimeZoneOffset = 9;
    public static readonly TimeSpan DefaultTimeZoneOffset = TimeSpan.FromHours(9);
    public const string LanguageCode = "ja";
    public const string LanguageEnCode = "en";
    public const string DefaultTimeZoneId = "Asia/Tokyo";
    public const string ServiceNameOfEmployee = "EmployeeService";
    public const string ServiceNameOfManager = "ManagerService";
    public const string ServiceNameOfSite = "SiteService";
    public const string ServiceNameOfUser = "UserService";

    public const int BookingSearchCheckOutDayOffset = 18;
    public const int BookingSearchDisplayCheckOutDayOffset = 13;
    public const int OnlinePaymentDayLimit = 150;
    public const int MaxRestNumber = 31;

    public static string ConvertPaymentTypeToJapanese(
        string paymentType
    )
    {
        return paymentType switch
        {
            nameof(PaymentTypes.OnSidePayment) => "現地決済",
            nameof(PaymentTypes.OnLinePayment) => "オンライン決済",
            _ => paymentType
        };
    }

    public static string ConvertReservationStatusToJapanese(
        string status
    )
    {
        return status switch
        {
            nameof(ReservationStatus.Confirmed) => "予約",
            nameof(ReservationStatus.Reserved) => "予約変更",
            nameof(ReservationStatus.Modified) => "予約変更",
            nameof(ReservationStatus.UserCanceled) => "キャンセル",
            nameof(ReservationStatus.GuestCanceled) => "キャンセル",
            nameof(ReservationStatus.ManagerCanceled) => "キャンセル",
            _ => status
        };
    }

    public static string ConvertCancellationPaymentTypeToJapanese(
       string paymentType
    )
    {
        return paymentType switch
        {
            nameof(PaymentTypes.OnSidePayment) => "振込（キャンセル料に関するご案内をご確認ください）",
            nameof(PaymentTypes.OnLinePayment) => "オンライン決済",
            _ => paymentType
        };
    }
}
