using CsvHelper.Configuration;

namespace Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;

public record SaleDetailResponseCsv
{
    public string? Code { get; set; }
    public string? State { get; set; }
    public bool IsReserved { get; set; }
    public DateTime? BookingDateTime { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int NumberOfNights { get; set; }
    public int NumberOfPeople { get; set; }
    public string? ReserverName { get; set; }
    public string? PaymentType { get; set; }
    public decimal TotalPrice { get; set; }
    public string? FacilityCode { get; set; }
    public string? FacilityName { get; set; }
    public string? FacilityRecordCode { get; set; }
    public bool? DayUse { get; set; }
}

public sealed class SaleDetailResponseCsvMap : ClassMap<SaleDetailResponseCsv>
{
    private const string DateTimeFormat = "yyyy年MM月dd日 HH時mm分ss秒";
    private const string DateFormat = "yyyy年MM月dd日";

    public SaleDetailResponseCsvMap()
    {
        Map(x => x.FacilityRecordCode)
            .Index(0)
            .Name("施設コード");
        Map(x => x.FacilityName)
            .Index(1)
            .Name("施設名");
        Map(x => x.Code)
            .Index(2)
            .Name("予約番号");
        Map(x => x.BookingDateTime)
            .Index(3)
            .Name("予約日")
            .TypeConverterOption.Format(DateTimeFormat);
        Map(x => x.CheckInDate)
            .Index(4)
            .Name("チェックイン日")
            .TypeConverterOption.Format(DateFormat);
        Map(x => x.CheckOutDate)
            .Index(5)
            .Name("チェックアウト日")
            .TypeConverterOption.Format(DateFormat);
        Map(x => x.NumberOfNights)
            .Index(6)
            .Name("泊数");
        Map(x => x.ReserverName)
            .Index(7)
            .Name("予約者");
        Map(x => x.NumberOfPeople)
            .Index(8)
            .Name("人数");
        Map(x => x.State)
            .Index(9)
            .Name("状態");
        Map(x => x.DayUse)
            .Index(10)
            .Name("日帰り・デイユース");
        Map(x => x.PaymentType)
            .Index(11)
            .Name("支払い区分");
        Map(x => x.TotalPrice)
            .Index(12)
            .Name("予約金額");
    }
}
