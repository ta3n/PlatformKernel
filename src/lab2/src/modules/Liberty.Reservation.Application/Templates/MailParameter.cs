namespace Liberty.Reservation.Application.Templates;

/// <summary>
/// Represents a mail template parameter with a key and description.
/// </summary>
/// <param name="Key">The unique key identifying the mail template parameter.</param>
/// <param name="Description">A description of the mail template parameter.</param>
public record MailTemplateParameter(
    string Key,
    string Description
);

public static class MailParameter
{
    public static readonly MailTemplateParameter ApplicationName = new(
        "applicationName",
        "予約システムの名前"
    );

    public static readonly MailTemplateParameter FacilityName = new(
        "facilityName",
        "予約した施設の名前"
    );

    public static readonly MailTemplateParameter Code = new(
        "code",
        "予約のコード"
    );

    public static readonly MailTemplateParameter Url = new(
        "url",
        "予約確認のURL"
    );

    public static readonly MailTemplateParameter ManagerUrl = new(
        "url",
        "施設管理画面のURL"
    );

    public static readonly MailTemplateParameter ManagementUrl = new(
        "ManagementUrl",
        "施設管理画面へのリンク"
    );

    public static readonly MailTemplateParameter UserUrl = new(
        "userUrl",
        "ユーザー管理画面へのリンク"
    );

    public static readonly MailTemplateParameter ReservationDetailUrl = new(
        "reservationDetailURL",
        "予約詳細ページへのリンク"
    );

    public static readonly MailTemplateParameter ReservationDateTime = new(
        "reservationDateTime",
        "予約受付日時"
    );

    public static readonly MailTemplateParameter CheckInDate = new(
        "CheckInDate",
        "チェックイン予定日"
    );

    public static readonly MailTemplateParameter CheckInTime = new(
        "CheckInTime",
        "チェックイン予定時刻"
    );

    public static readonly MailTemplateParameter RestNumber = new(
        "RestNumber",
        "宿泊数"
    );

    public static readonly MailTemplateParameter CheckOutDate = new(
        "CheckOutDate",
        "チェックアウト予定日"
    );

    public static readonly MailTemplateParameter CheckOutTime = new(
        "CheckOutTime",
        "チェックアウト予定時刻"
    );

    public static readonly MailTemplateParameter RoomNumber = new(
        "RoomNumber",
        "部屋数"
    );

    public static readonly MailTemplateParameter TotalPersons = new(
        "TotalPersons",
        "宿泊人数の詳細"
    );

    public static readonly MailTemplateParameter RoomGroupName = new(
        "RoomGroupName",
        "部屋タイプ"
    );

    public static readonly MailTemplateParameter PlanName = new(
        "PlanName",
        "プラン名"
    );

    public static readonly MailTemplateParameter MealType = new(
        "MealType",
        "食事の種類"
    );

    public static readonly MailTemplateParameter MainUserName = new(
        "MainUserName",
        "宿泊者氏名"
    );

    public static readonly MailTemplateParameter MainUserTel = new(
        "MainUserTel",
        "宿泊者連絡先"
    );

    public static readonly MailTemplateParameter MainUserAdress = new(
        "MainUserAddress",
        "宿泊者の住所"
    );

    public static readonly MailTemplateParameter ReserverName = new(
        "ReserverName",
        "予約者氏名"
    );

    public static readonly MailTemplateParameter ReserverEmail = new(
        "ReserverEmail",
        "予約者メールアドレス"
    );

    public static readonly MailTemplateParameter ReserverAddress = new(
        "ReserverAddress",
        "予約者の住所"
    );

    public static readonly MailTemplateParameter Questions = new(
        "questions",
        "質問内容"
    );

    public static readonly MailTemplateParameter Memo = new(
        "Memo",
        "備考"
    );

    public static readonly MailTemplateParameter PayOff = new(
        "payOff",
        "精算情報"
    );

    public static readonly MailTemplateParameter FirstRestMaleNumber = new(
        "1stRestMaleNumber",
        "男性の人数（大人）"
    );

    public static readonly MailTemplateParameter FirstRestFemaleNumber = new(
        "1stRestFemaleNumber",
        "女性の人数（大人）"
    );

    public static readonly MailTemplateParameter FirstRestChildNumber = new(
        "1stRestChildNumber",
        "子供・幼児の人数"
    );

    public static readonly MailTemplateParameter ReservationDetail = new(
        "reservationDetail",
        "予約明細"
    );

    public static readonly MailTemplateParameter FacilityAddress = new(
        "facilityAddress",
        "施設の住所"
    );

    public static readonly MailTemplateParameter FacilityTel = new(
        "facilityTel",
        "施設の電話番号"
    );

    public static readonly MailTemplateParameter FacilityAccessInfo = new(
        "facilityAccessInfo",
        "アクセス"
    );

    public static readonly MailTemplateParameter FacilityAddressGoogleMap = new(
        "facilityAddressGoogleMap",
        "施設住所のGoogle Mapリンク"
    );

    public static readonly MailTemplateParameter CancellationDaysBeforeFee = new(
        "cancellationDaysBeforeFee",
        "キャンセル料が発生するチェックイン前の日数を指定する変数"
    );

    public static readonly MailTemplateParameter CancellingDescription = new(
        "cancellingDescription",
        "キャンセルについての説明"
    );

    public static readonly MailTemplateParameter CancellationInfo = new(
        "cancellationInfo",
        "キャンセル情報"
    );

    public static readonly MailTemplateParameter CancelRuleName = new(
        "cancelRuleName",
        "キャンセル規定の名称"
    );

    public static readonly MailTemplateParameter ManagerCanceledDescription = new(
        "managerCanceledDescription",
        "施設管理者によるキャンセルに関する説明:「この予約は施設によってキャンセルされました。」それ以外の場合、その文書が表示されないことです。"
    );

    public static readonly MailTemplateParameter ManagerEditedDescription = new(
        "managerEditedDescription",
        "施設管理者による変更に関する説明:「この予約は施設によって変更されました。」それ以外の場合、その文書が表示されないことです。"
    );

    public static readonly MailTemplateParameter Actor = new(
        "actor",
        "キャンセル手続きを行ったユーザまたは役職(施設管理者、予約者)"
    );

    public static readonly MailTemplateParameter DayLeft = new(
        "dayLeft",
        "予約日までの日数"
    );

    public static readonly MailTemplateParameter CancelPeriod = new(
        "cancelPeriod",
        "キャンセル料が発生する期間（予約日何日前）"
    );

    public static readonly MailTemplateParameter ReserverNameKana = new(
        "ReserverNameKana",
        "予約者氏名（カナ）"
    );

    public static readonly MailTemplateParameter ReserverTel = new(
        "ReserverTel",
        "予約者連絡先"
    );

    public static readonly MailTemplateParameter ReserverAddressCountry = new(
        "ReserverAddressCountry",
        "予約者の住所の国"
    );

    public static readonly MailTemplateParameter ReserverAddressPostalCode = new(
        "ReserverAddressPostalCode",
        "予約者の住所の郵便番号"
    );

    public static readonly MailTemplateParameter ReserverAddressPrefecture = new(
        "ReserverAddressPrefecture",
        "予約者の住所の都道府県"
    );

    public static readonly MailTemplateParameter ReserverAddressCity = new(
        "ReserverAddressCity",
        "予約者の住所の市区町村"
    );

    public static readonly MailTemplateParameter ReserverAddressStreet = new(
        "ReserverAddressStreet",
        "予約者の住所の番地等"
    );

    public static readonly MailTemplateParameter ReserverGender = new(
        "ReserverGender",
        "予約者の性別"
    );
}
