using Liberty.Fax.Models;

namespace Liberty.Fax.ErrorCodeFax;

public static class FaxErrors
{
    public static readonly List<FaxErrorItem> FaxErrorItems =
    [
        #region 共通 (Common)

        new()
        {
            Code = "000000",
            Message = "正常完了"
        },
        new()
        {
            Code = "001001",
            Message = "認証情報不正"
        },
        new()
        {
            Code = "001100",
            Message = "フォーマット不正"
        },
        new()
        {
            Code = "009999",
            Message = "想定外エラー"
        },

        #endregion

        #region 受信 (Receive)

        new()
        {
            Code = "012001",
            Message = "検索条件不正：絞り込み条件不足"
        },
        new()
        {
            Code = "012002",
            Message = "検索条件不正：指定期間不正"
        },
        new()
        {
            Code = "012003",
            Message = "検索条件過多"
        },
        new()
        {
            Code = "013001",
            Message = "指定件数過多"
        },

        #endregion

        #region 送信 (Send)

        new()
        {
            Code = "021001",
            Message = "依頼件数過多"
        },
        new()
        {
            Code = "021002",
            Message = "送信先FAX番号：同一番号が指定"
        },
        new()
        {
            Code = "021003",
            Message = "送信先FAX番号：数値項目以外が入力"
        },
        new()
        {
            Code = "021004",
            Message = "送信先FAX番号：桁数オーバー"
        },
        new()
        {
            Code = "021005",
            Message = "ユーザーキー：桁数オーバー"
        },
        new()
        {
            Code = "021006",
            Message = "TSI：桁数オーバー"
        },
        new()
        {
            Code = "021007",
            Message = "ヘッダー情報：桁数オーバー"
        },
        new()
        {
            Code = "021008",
            Message = "メールアドレス：桁数オーバー"
        },
        new()
        {
            Code = "021009",
            Message = "件名：桁数オーバー"
        },
        new()
        {
            Code = "021010",
            Message = "本文：桁数オーバー"
        },
        new()
        {
            Code = "021011",
            Message = "添付書類なし"
        },
        new()
        {
            Code = "021013",
            Message = "添付書類：桁数オーバー"
        },
        new()
        {
            Code = "021014",
            Message = "送信結果エラー"
        },
        new()
        {
            Code = "022001",
            Message = "検索条件不正：絞り込み条件不正"
        },
        new()
        {
            Code = "022002",
            Message = "検索条件不正：指定期間不正"
        },
        new()
        {
            Code = "022003",
            Message = "指定件数過多"
        },
        new()
        {
            Code = "022004",
            Message = "ユーザーキー：桁数オーバー"
        },
        new()
        {
            Code = "022005",
            Message = "明細番号：桁数オーバー"
        },
        new()
        {
            Code = "022006",
            Message = "結果種別数過多"
        },
        new()
        {
            Code = "023001",
            Message = "指定件数過多"
        },

        #endregion
    ];
}
