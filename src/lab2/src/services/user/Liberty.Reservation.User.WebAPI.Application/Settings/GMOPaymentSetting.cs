namespace Liberty.Reservation.User.WebAPI.Application.Settings;

public class GMOPaymentSetting
{
    public string? Url { get; set; }
    public string? PaymentHost { get; set; }

    public int UseCredit { get; set; }
    public string? ShopId { get; set; }
    public string? ShopPassword { get; set; }
    public string? JobCd { get; set; }

    public string? OrderDateFormat { get; set; }
    public int Expire { get; set; }
    public int Tax { get; set; }
    public string? RedirectUserUrl { get; set; }
    public string? RedirectSiteUrl { get; set; }
}
