using Liberty.Entity;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

/// <summary>
/// GMOペイメントオンライン決済結果通知
/// </summary>
public class GmoPaymentResultRequest : EntityData
{
    /// <summary>1 ショップID</summary>
    public string? ShopId { get; set; }

    /// <summary>2ショップパスワード</summary>
    public string? ShopPass { get; set; }

    /// <summary>3 取引 ID</summary>
    public string? AccessId { get; set; }

    /// <summary>4 取引パスワード</summary>
    public string? AccessPass { get; set; }

    /// <summary>5 オーダーID</summary>
    public string? OrderId { get; set; }

    /// <summary>6 現状態</summary>
    public string? Status { get; set; }

    /// <summary>7  処理区分</summary>
    public string? JobCd { get; set; }

    /// <summary>8  利用金額</summary>
    public string? Amount { get; set; }

    /// <summary>9  税送料</summary>
    public string? Tax { get; set; }

    /// <summary>10  通貨コード</summary>
    public string? Currency { get; set; }

    /// <summary>11  仕向先会社コード</summary>
    public string? Forward { get; set; }

    /// <summary>12  支払方法</summary>
    public string? Method { get; set; }

    /// <summary>13  支払回数</summary>
    public string? PayTimes { get; set; }

    /// <summary>14  トランザクション ID</summary>
    public string? TranId { get; set; }

    /// <summary>15  承認番号</summary>
    public string? Approve { get; set; }

    /// <summary>16  処理日時</summary>
    public string? TranDate { get; set; }

    /// <summary>17  エラーコード</summary>
    public string? ErrCode { get; set; }

    /// <summary>18  エラー詳細コード</summary>
    public string? ErrInfo { get; set; }

    /// <summary>19  決済方法</summary>
    public string? PayType { get; set; }

    /// <summary>
    /// GMOペイメントの処理が完了しているか？
    /// </summary>
    public bool IsCompleted => !string.IsNullOrEmpty(Approve);

    // クレジットカード決済_1_04.pdf P70 より参照
    /// <summary>未決済</summary>
    public bool IsUnprocessed => Status is nameof(GmoPaymentResultRequestStatus.UNPROCESSED);

    /// <summary>未決済(3DS登録済)</summary>
    public bool IsAuthenticated => Status is nameof(GmoPaymentResultRequestStatus.AUTHENTICATED);

    /// <summary>有効性チェック</summary>
    public bool IsCheck => Status is nameof(GmoPaymentResultRequestStatus.CHECK);

    /// <summary>即時売上</summary>
    public bool IsCapture => Status is nameof(GmoPaymentResultRequestStatus.CAPTURE);

    /// <summary>仮売上</summary>
    public bool IsAuth => Status is nameof(GmoPaymentResultRequestStatus.AUTH);

    /// <summary>実売上</summary>
    public bool IsSales => Status is nameof(GmoPaymentResultRequestStatus.SALES);

    /// <summary>取消</summary>
    public bool IsVoid => Status is nameof(GmoPaymentResultRequestStatus.VOID);

    /// <summary>返品</summary>
    public bool IsReturn => Status is nameof(GmoPaymentResultRequestStatus.RETURN);

    /// <summary>月跨り返品</summary>
    public bool IsReturnX => Status is nameof(GmoPaymentResultRequestStatus.RETURNX);

    /// <summary>簡易オーソリ</summary>
    public bool IsSauth => Status is nameof(GmoPaymentResultRequestStatus.SAUTH);

    public ICollection<OrderGmoPaymentResultRequest>? OrderGmoPaymentResultRequests { get; set; }
}
