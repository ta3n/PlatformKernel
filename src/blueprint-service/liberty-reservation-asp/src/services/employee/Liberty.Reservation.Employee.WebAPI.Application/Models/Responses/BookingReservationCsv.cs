using CsvHelper.Configuration;

namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;

public record BookingReservationCsv
{
    public string? FacilityCode { get; set; }
    public string? FacilityName { get; set; }
    public string? Code { get; set; }
    public string? SiteName { get; set; }
    public string? SiteCode { get; set; }
    public string? ParentCode { get; set; }
    public string? GmoOrderId { get; set; }
    public string? State { get; set; }
    public DateTime? BookingDateTime { get; set; }
    public DateTime? ConfirmDateTime { get; set; }
    public DateTime? CancelledDateTime { get; set; }
    public DateTime? CheckInDate { get; set; }
    public int LengthOfStay { get; set; }
    public int NumberOfRooms { get; set; }
    public string? ReserverName { get; set; }
    public string? PaymentType { get; set; }
    public bool IsNoShow { get; set; }
    public DateTime? CheckOutDate { get; set; }
    public decimal AllTotalPrice { get; set; }
    public DateTime? NoShowDateTime { get; set; }
    public string? AccessID { get; set; }
}

public sealed class BookingReservationCsvMap : ClassMap<BookingReservationCsv>
{
    private const string DateTimeFormat = "yyyy年MM月dd日 HH時mm分ss秒";
    private const string DateFormat = "yyyy年MM月dd日";

    public BookingReservationCsvMap()
    {
        Map(x => x.Code)
            .Index(0)
            .Name("予約番号");

        Map(x => x.ParentCode)
            .Index(1)
            .Name("親（大元）の予約番号");

        Map(x => x.FacilityCode)
            .Index(2)
            .Name("施設コード");

        Map(x => x.FacilityName)
            .Index(3)
            .Name("施設名");

        Map(x => x.State)
            .Index(4)
            .Name("状態");

        Map(x => x.BookingDateTime)
            .Index(5)
            .Name("予約日")
            .TypeConverterOption.Format(DateTimeFormat);

        Map(x => x.ConfirmDateTime)
            .Index(6)
            .Name("認証日")
            .TypeConverterOption.Format(DateTimeFormat);

        Map(x => x.CancelledDateTime)
            .Index(7)
            .Name("キャンセル日時")
            .TypeConverterOption.Format(DateTimeFormat);

        Map(x => x.CheckInDate)
            .Index(8)
            .Name("チェックイン日")
            .TypeConverterOption.Format(DateFormat);

        Map(x => x.LengthOfStay)
            .Index(9)
            .Name("宿泊期間");

        Map(x => x.NumberOfRooms)
            .Index(10)
            .Name("部屋数");

        Map(x => x.ReserverName)
            .Index(11)
            .Name("予約者");

        Map(x => x.PaymentType)
            .Index(12)
            .Name("支払い区分");

        Map(x => x.IsNoShow)
            .Index(13)
            .Name("Noshow")
            .TypeConverterOption.BooleanValues(true, true, "はい")
            .TypeConverterOption.BooleanValues(false, true, "いいえ");

        Map(x => x.CheckOutDate)
            .Index(14)
            .Name("チェックアウト日")
            .TypeConverterOption.Format(DateFormat);

        Map(x => x.AllTotalPrice)
            .Index(15)
            .Name("予約金額");

        Map(x => x.GmoOrderId)
            .Index(16)
            .Name("オンライン決済(GMOPG): OrderId");

        Map(x => x.AccessID)
            .Index(17)
            .Name("オンライン決済(GMOPG)取引ID: job_cd");

        Map(x => x.NoShowDateTime)
           .Index(18)
           .Name("noshow処理日時")
           .TypeConverterOption.Format(DateTimeFormat);

        Map(x => x.SiteCode)
           .Index(19)
           .Name("掲載先コード");

        Map(x => x.SiteName)
           .Index(20)
           .Name("掲載先名");
    }
}
