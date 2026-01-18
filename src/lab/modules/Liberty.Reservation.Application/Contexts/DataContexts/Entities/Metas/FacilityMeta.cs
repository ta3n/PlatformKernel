using Liberty.Reservation.Application.Constants;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;

public class FacilityMeta
{
    public int? RoomNumberWesternStyle { get; set; }
    public int? RoomNumberJapaneseStyle { get; set; }
    public int? RoomNumberJapaneseWesternStyle { get; set; }
    public int? RoomNumberOtherStyle { get; set; }

    /// <summary>
    /// 宿泊受付期限（時）
    /// </summary>
    public TimeSpan ReceptionLimit { get; set; }

    /// <summary>
    /// チェックイン開始時刻
    /// </summary>
    public TimeSpan CheckInStart { get; set; }

    /// <summary>
    /// チェックイン終了時刻
    /// </summary>
    public TimeSpan CheckInEnd { get; set; }

    /// <summary>
    /// チェックアウト時刻
    /// </summary>
    public TimeSpan CheckOut { get; set; }

    /// <summary>
    /// 見出し1
    /// </summary>
    public string? Heading1 { get; set; }

    /// <summary>
    /// PRポイント
    /// </summary>
    public string? PRPointComment { get; set; }

    /// <summary>
    /// キャンセルについて
    /// </summary>
    public string? CancellingComment { get; set; }

    public string? CancellingTable { get; set; }

    public string? MapUrl { get; set; }

    /// <summary>
    /// 入湯税有無
    /// </summary>
    public bool UseSpaTax { get; set; }

    /// <summary>
    /// 入湯税に関する補足
    /// </summary>
    public string? SpaTaxComment { get; set; }

    public string? SpaTaxTable { get; set; }

    public string? SpaType { get; set; }

    public string? SpaName { get; set; }

    public string? SpaDescription { get; set; }

    /// <summary>
    /// 風呂・温泉に対する補足
    /// </summary>
    public string? SpaInfoComment { get; set; }

    /// <summary>
    /// 子供受け入れ
    /// </summary>
    public bool IsAcceptChildren { get; set; }

    /// <summary>
    /// 子供受け入れ補足
    /// </summary>
    public string? AcceptChildrenInfoComment { get; set; }

    /// <summary>
    /// ペット受け入れ
    /// </summary>
    public bool IsAcceptPet { get; set; }

    /// <summary>
    /// ペット受け入れ補足
    /// </summary>
    public string? AcceptPetInfoComment { get; set; }

    /// <summary>
    /// バリアフリー
    /// </summary>
    public bool IsBarrierFree { get; set; }

    /// <summary>
    /// バリアフリー補足
    /// </summary>
    public string? BarrierFreeInfoComment { get; set; }

    /// <summary>
    /// 料理区分コメント、料理に関するご案内
    /// </summary>
    public string? MealTypeComment { get; set; }

    /// <summary>
    /// 現地決済補足、現地決済に関するご案内
    /// </summary>
    public string? OnSidePaymentComment { get; set; }

    /// <summary>
    /// オンライン決済補足
    /// </summary>
    public string? OnLinePaymentComment { get; set; }

    /// <summary>
    /// 料金特記
    /// </summary>
    public string? PaymentComment { get; set; }

    /// <summary>
    /// アクセス概要、説明
    /// </summary>
    public string? AccessInfoComment { get; set; }

    /// <summary>
    /// 最寄り駅
    /// </summary>
    public string? NearStationInfoComment { get; set; }

    public bool ExistsParking { get; set; }

    /// <summary>
    /// 駐車場補足
    /// </summary>
    public string? ParkingInfoComment { get; set; }

    /// <summary>
    /// 送迎可か？
    /// </summary>
    public bool CanTransfer { get; set; }

    /// <summary>
    /// 送迎補足
    /// </summary>
    public string? TransferComment { get; set; }

    /// <summary>
    /// 施設設備に対する補足
    /// </summary>
    public string? EquipmentInfoComment { get; set; }

    /// <summary>
    /// 部屋特徴に対する補足
    /// </summary>
    public string? RoomInfoComment { get; set; }

    /// <summary>
    /// アメニティに対する補足
    /// </summary>
    public string? AmenityInfoComment { get; set; }

    /// <summary>
    /// サービス・レジャーに対する補足
    /// </summary>
    public string? LeisureInfoComment { get; set; }

    /// <summary>
    /// よくある質問
    /// </summary>
    public string? FAQInfoComment { get; set; }

    /// <summary>
    /// その他ご案内
    /// </summary>
    public string? OtherInfoComment { get; set; }

    /// <summary>
    /// 指定金額より下回ったら警告を使用するか
    /// </summary>
    public bool UseWarnPrice { get; set; }

    /// <summary>
    /// 指定金額より下回ったら警告
    /// </summary>
    public int WarnPrice { get; set; }

    /// <summary>
    /// 基準金額より下回ったら警告を使用するか
    /// </summary>
    public bool UseWarnPricePercent { get; set; }

    /// <summary>
    /// 基準金額より下回ったら警告
    /// </summary>
    public int WarnPricePercent { get; set; }

    /// <summary>
    /// 管理画面操作モード
    /// </summary>
    public OperationModes OperationMode { get; set; }

    /// <summary>
    /// ファイルストレージ制限
    /// </summary>
    public float FileStorageLimit { get; set; }

    /// <summary>
    /// インポート用プラン名を使用
    /// </summary>
    public bool UseImportPlanName { get; set; }

    /// <summary>
    /// 検索係数
    /// </summary>
    public float SEOCoefficient { get; set; }

    /// <summary>
    /// システムから送られるメールの宛先
    /// </summary>
    public string? SystemEMail { get; set; }
}
