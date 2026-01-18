using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Models;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;

public class BookingData
{
    public string? LanguageCode { get; set; }
    public required FacilityData Facility { get; set; }

    public required PlanData Plan { get; set; }

    public required RoomGroupData RoomGroup { get; set; }

    public required SiteData Site { get; set; }

    public List<QuestionData>? PlanQuestions { get; set; }
    public List<QuestionData>? OptionQuestions { get; set; }
    public List<BookingAppDateData>? AppDates { get; set; }
    public List<PersonAgeTypeData>? PersonAgeTypes { get; set; }
    public SendMailState SendMailState { get; set; } = new();

    public decimal TotalRoomPrice { get; set; }
    public decimal TotalSpaTax { get; set; }
    public decimal TotalOptionPrice { get; set; }
    public int UsedPoint { get; set; }
    public decimal TotalDiscount => UsedPoint;
    public decimal AllTotalPrice => TotalPrice - TotalDiscount;
    public decimal TotalPrice => TotalRoomPrice + TotalSpaTax + TotalOptionPrice;
    public bool IsSiteLocation { get; set; }
    public int TimeZoneOffset { get; set; }
    public string? MediaCode { get; set; }

    public string? RootCode { get; set; }

    public bool? UseSpaTax { get; set; }

    public List<QuestionData> GetAllQuestions()
    {
        var questions = new List<QuestionData>();

        if (PlanQuestions is { Count: > 0 })
        {
            questions.AddRange(PlanQuestions);
        }

        if (OptionQuestions is { Count: > 0 })
        {
            questions.AddRange(OptionQuestions);
        }

        return questions;
    }
}

public class FacilityData
{
    public long? Id { get; set; }
    public string? Name { get; set; }
    public string? AccessInfoComment { get; set; }
    public MultilingualText? LocalizedNames { get; set; }
    public string? Address => Address1 + Address2 + Address3 + Address4;
    public string? FullAddress => string.Join(", ", new[] { Address1, Address2, Address3, Address4 }.Where(a => !string.IsNullOrEmpty(a)));
    public string? Address1 { get; set; }
    public string? Address2 { get; set; }
    public string? Address3 { get; set; }
    public string? Address4 { get; set; }
    public string? TimeZone { get; set; }
    public string? TimeZoneId { get; set; }
    public string? Tel { get; set; }
    public bool? CanOnLinePayment { get; set; }
    public bool? IsOnSidePayment { get; set; }
    public bool? IsOnLinePayment { get; set; }
    public bool CanAddRoomOnModify { get; set; }
    public bool IsExtendedStayOnModify { get; set; }
}

public class PlanData
{
    public long? Id { get; set; }
    public string? Name { get; set; }
    public bool? IsOnSidePayment { get; set; }
    public bool? IsOnLinePayment { get; set; }
    public bool DayUse { get; set; }
    public PlanMeta? Meta { get; set; }
    public List<PlanMealData>? Meals { get; set; }
    public List<FileData>? Files { get; set; }
    public bool IsCancelSameAccept { get; set; }
    public int? CancelDayLimit { get; init; }
    public int? ReceptionDayLimit { get; init; }
    public TimeSpan? CancelLimit { get; init; }
    public BookingCancellationPolicyModel? CancellationDataPolicy { get; set; }
}

public class PlanMealData
{
    public long? Id { get; set; }
    public string? Name { get; set; }
    public MealTypeEatTypes MealTypeEatType { get; set; }
}

public class RoomGroupData
{
    public long? Id { get; set; }
    public string? Name { get; set; }
    public string? GroupName { get; set; }
    public bool? IsEnabledSmoking { get; set; }
    public int? CapacityMax { get; set; }
    public int? CapacityMin { get; set; }
    public List<FileData>? Files { get; set; }
}

public class SiteData
{
    public long? Id { get; set; }
    public string? Name { get; set; }
    public MultilingualText? LocalizedNames { get; set; }
    public string? ShortName { get; set; }
    public string? PrefixName { get; set; }
}

public class BookingAppDateData
{
    public long AppDateId { get; set; }
    public int RestIndex { get; set; }
    public decimal Price { get; set; }
    public decimal SpaTax { get; set; }
    public decimal OptionPrice { get; set; }
    public decimal TotalPrice => Price + OptionPrice;
    public decimal TotalPriceAndSpa => TotalPrice + SpaTax;
    public required IEnumerable<BookingRoomDataOfAppDate> Rooms { get; set; }
}

public class BookingRoomDataOfAppDate
{
    public int RoomIndex { get; set; }
    public decimal RoomPrice { get; set; }
    public IEnumerable<PeoplePriceDataOfRoom> PricePeoples { get; set; } = [];
    public IEnumerable<OptionItemDataOfRoom>? OptionItems { get; set; }
    public CustomerData? CustomerInfo { get; set; }
    public decimal SpaTax { get; set; }
    public decimal TotalOptionPrice { get; set; }
    public int Persons { get; set; }

    /// <summary>
    /// 人数
    /// </summary>
    public int? MalePersons { get; set; }

    /// <summary>
    /// 人数
    /// </summary>
    public int? FemalePersons { get; set; }
}

public class PeoplePriceDataOfRoom
{
    public decimal RoomPrice { get; set; }
    public decimal SpaTax { get; set; }

    public decimal TotalPrice { get; set; }
    public decimal TotalSpaTax { get; set; }

    public int Persons { get; set; }

    /// <summary>
    /// 人数
    /// </summary>
    public int? MalePersons { get; set; }

    /// <summary>
    /// 人数
    /// </summary>
    public int? FemalePersons { get; set; }

    /// <summary>
    /// 人数
    /// </summary>
    public int? NonePersons { get; set; }

    public int? OtherPersons { get; set; }

    public required PeopleDataOfRoom PersonAgeType { get; set; }
}

public class PeopleDataOfRoom
{
    public long? Id { get; set; }

    public string? Name { get; set; }
    public bool? IsMain { get; set; }

    /// <summary>
    /// 年齢上限
    /// </summary>
    public int? AgeMax { get; set; }

    /// <summary>
    /// 年齢下限
    /// </summary>
    public int? AgeMin { get; set; }
}

public class OptionItemDataOfRoom
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public decimal? Price { get; set; }

    /// <summary>
    /// 数量
    /// </summary>
    public int Number { get; set; }

    /// <summary>
    /// 小計
    /// </summary>
    public decimal TotalPrice => (Price ?? 0) * Number;
}

public class QuestionData
{
    public long? Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? FormData { get; set; }

    /// <summary>
    /// 回答データ
    /// </summary>
    public string? AnswerData { get; set; }

    public bool? IsRequired { get; set; }
}

public class FileData
{
    public string? Code { get; set; }

    /// <summary>
    /// コンテントタイプ
    /// </summary>
    public string? ContentType { get; set; }
}

public class CustomerData
{
    /// <summary>
    /// 名前
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// かな
    /// </summary>
    public string? Kana { get; set; }
}

public class SendMailState
{
    public bool? BookingReminderOfUpcomingCheckInDateSend { get; set; }
    public bool? BookingReminderOfUpcomingCheckInDateSent { get; set; }
    public bool? BookingCancellationFeeReminderSend { get; set; }
    public bool? BookingCancellationFeeReminderSent { get; set; }
}

public class PersonAgeTypeData
{
    public long Id { get; set; }

    public string? Name { get; set; }

    /// <summary>
    /// 大人〇名として料金指定の際のメインとなる区分としてあつかうか？
    /// </summary>
    public bool IsMain { get; set; }

    /// <summary>
    /// 年齢上限
    /// </summary>
    public int? AgeMax { get; set; }

    /// <summary>
    /// 年齢下限
    /// </summary>
    public int? AgeMin { get; set; }

    public IEnumerable<SpaTaxOfPersonAgeTypeData>? PersonAgeTypeSpaTaxDatas { get; set; }
}

public class SpaTaxOfPersonAgeTypeData
{
    /// <summary>
    /// 金額上限
    /// </summary>
    public int? PriceMax { get; set; }

    /// <summary>
    /// 金額下限
    /// </summary>
    public int? PriceMin { get; set; }

    /// <summary>
    /// 入湯税
    /// </summary>
    public int? Tax { get; set; }
}
