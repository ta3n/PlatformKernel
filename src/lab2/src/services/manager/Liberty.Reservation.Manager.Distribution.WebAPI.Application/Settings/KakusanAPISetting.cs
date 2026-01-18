namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Settings;

public class KakusanApiSetting
{
    public C001Setting? C001 { get; set; }
    public C002Setting? C002 { get; set; }
    public C003Setting? C003 { get; set; }
    public C004Setting? C004 { get; set; }
}

public class C001Setting
{
    public int HotelCountMin { get; set; } = 1;
    public int HotelCountMax { get; set; } = 10;
}

public class C002Setting
{
    public int HotelCountMin { get; set; } = 1;
    public int HotelCountMax { get; set; } = 10;
    public int MaxFromDays { get; set; } = 366;
    public int RangeDaysMin { get; set; }
    public int RangeDaysMax { get; set; } = 90;
}

public class C003Setting
{
    public int HotelCountMin { get; set; } = 1;
    public int HotelCountMax { get; set; } = 10;
    public int MaxFromDays { get; set; } = 366;
    public int RangeDaysMin { get; set; } = 0;
    public int RangeDaysMax { get; set; } = 32;
}

public class C004Setting
{
    public int HotelCountMin { get; set; } = 1;
    public int HotelCountMax { get; set; } = 10;
    public int DaysMin { get; set; } = 1;
    public int DaysMax { get; set; } = 100;
}
