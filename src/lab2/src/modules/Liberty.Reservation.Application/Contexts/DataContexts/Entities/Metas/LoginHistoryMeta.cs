namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;

public class LoginHistoryMeta
{
    /// <summary>
    /// WebAPI側で取得されたIP
    /// </summary>
    public string? RemoteIpAddress { get; set; }

    /// <summary>
    /// IPV4
    /// </summary>
    public string? ClientIpAddress { get; set; }

    /// <summary>
    /// デバイス
    /// </summary>
    public string? ClientDevice { get; set; }

    /// <summary>
    /// リクエスト日付
    /// </summary>
    public string? RequestDate { get; set; }

    /// <summary>
    /// 戻り先URLの指定,アクセス元URL
    /// </summary>
    public string? ReturnUrl { get; set; }
}
