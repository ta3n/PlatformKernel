using Liberty.Entity;
using Liberty.Entity.Auditing;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Newtonsoft.Json;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

/// <summary>
/// ログイン履歴
/// </summary>
public class LoginHistory : EntityData, ILoginHistory
{
    /// <summary>
    /// 日付
    /// </summary>
    public DateTime DateTime { get; set; }

    /// <summary>
    /// 情報JSON
    /// </summary>
    public string MetaJson { get; set; } = "{}";

    public LoginHistoryMeta? Meta
    {
        get
        {
            try
            {
                return JsonConvert.DeserializeObject<LoginHistoryMeta>(MetaJson);
            }
            catch
            {
                // 変換できない場合、新しいオブジェクトを返す
                return new LoginHistoryMeta();
            }
        }
        set => MetaJson = JsonConvert.SerializeObject(value);
    }

    // ILoginHistoryの項目
    public string? Device => Meta?.ClientDevice;
    public string? IpAddress => Meta?.ClientIpAddress;
    public bool? IsMobile => null;

    public ICollection<ApplicationUserLoginHistory>? ApplicationUserLoginHistories { get; set; }
}
