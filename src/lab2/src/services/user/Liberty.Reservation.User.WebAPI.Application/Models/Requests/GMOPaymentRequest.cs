using System.Text.Json.Serialization;

namespace Liberty.Reservation.User.WebAPI.Application.Models.Requests;

public record GmoPaymentRequest
{
    /// <summary>1 ショップID</summary>
    [JsonPropertyName("ShopID")]
    public string? ShopId { get; init; }

    /// <summary>2ショップパスワード</summary>
    public string? ShopPass { get; init; }

    /// <summary>3 取引 ID</summary>

    [JsonPropertyName("AccessID")]
    public string? AccessId { get; init; }

    /// <summary>4 取引パスワード</summary>
    public string? AccessPass { get; init; }

    /// <summary>5 オーダーID</summary>
    [JsonPropertyName("OrderID")]
    public string? OrderId { get; init; }

    /// <summary>6 現状態</summary>
    public string? Status { get; init; }

    /// <summary>7  処理区分</summary>
    public string? JobCd { get; init; }

    /// <summary>8  利用金額</summary>
    public string? Amount { get; init; }

    /// <summary>9  税送料</summary>
    public string? Tax { get; init; }

    /// <summary>10  通貨コード</summary>
    public string? Currency { get; init; }

    /// <summary>11  仕向先会社コード</summary>
    public string? Forward { get; init; }

    /// <summary>12  支払方法</summary>
    public string? Method { get; init; }

    /// <summary>13  支払回数</summary>
    public string? PayTimes { get; init; }

    /// <summary>14  トランザクション ID</summary>
    [JsonPropertyName("TranID")]
    public string? TranId { get; init; }

    /// <summary>15  承認番号</summary>
    public string? Approve { get; init; }

    /// <summary>16  処理日時</summary>
    public string? TranDate { get; init; }

    /// <summary>17  エラーコード</summary>
    public string? ErrCode { get; init; }

    /// <summary>18  エラー詳細コード</summary>
    public string? ErrInfo { get; init; }

    /// <summary>19  決済方法</summary>
    public string? PayType { get; init; }
}

public record GmoPaymentLinkPlusRequest(
    string Result
);

public class TransactionResultLinkPlus
{
    public string? AccessID { get; set; }
    public string? AccessPass { get; set; }
    public string? OrderID { get; set; }
    public string? Result { get; set; }
    public string? Processdate { get; set; }
    public string? ErrCode { get; set; }
    public string? ErrInfo { get; set; }
    public string? Paymethod { get; set; }
}

public class CreditLinkPlus
{
    public string? Status { get; set; }
    public string? Forward { get; set; }
    public string? Method { get; set; }
    public string? PayTimes { get; set; }
    public string? TranID { get; set; }
    public string? Approve { get; set; }
    public string? TranDate { get; set; }
}

public class GmoPaymentLinkPlusResponse
{
    [JsonPropertyName("transactionresult")]
    public TransactionResultLinkPlus? Transactionresult { get; set; }

    public CreditLinkPlus? Credit { get; set; }
}
