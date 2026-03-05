namespace Liberty.Reservation.Manager.Application.Models;

public class BookingReservationPriceData
{
    public long AppDateId { get; set; }

    public int RoomGroupIndex { get; set; }

    public decimal? Price { get; set; }

    public decimal? SpaTax { get; set; }

    public decimal? TotalSpaTax { get; set; }

    public decimal? TotalPrice { get; set; }

    /// <summary>
    /// 男性人数
    /// </summary>
    public int? MalePersons { get; set; }

    /// <summary>
    /// 女性人数
    /// </summary>
    public int? FemalePersons { get; set; }

    /// <summary>
    /// 性別未設定人数
    /// </summary>
    public int? NonePersons { get; set; }

    /// <summary>
    /// 合計人数
    /// 男性人数、女性人数とは別に設定されます
    /// </summary>
    public int Persons { get; set; }

    public PersonAgeTypeOfBookingReservationPriceData? PersonAgeType { get; set; }

    /// <summary>
    /// 合計人数が男性人数・女性人数の合計と一致しているか？
    /// 男性人数・女性人数のいずれが不明(=null)の場合、結果はbool?=nullを返します
    /// </summary>
    public bool? IsPersonsMatch
    {
        get
        {
            var malePersons = MalePersons;
            var femalePersons = FemalePersons;
            var persons = Persons;
            if (malePersons == null || femalePersons == null)
            {
                return null;
            }

            return persons == malePersons.Value + femalePersons.Value;
        }
    }
}

public class PersonAgeTypeOfBookingReservationPriceData
{
    public long Id { get; set; }
    public int? AgeMax { get; set; }
    public string? Name { get; set; }
    public int? AgeMin { get; set; }
    public bool IsMain { get; set; }
    public bool IsEnabled { get; set; }
    public string? PriceSettingType { get; set; }
    public float? Value { get; set; }
}
