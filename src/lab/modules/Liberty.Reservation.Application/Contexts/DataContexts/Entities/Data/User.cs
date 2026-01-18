using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

/// <summary>
/// アプリケーションユーザ
/// </summary>
public class User : EntityData
{
    public long UserInfoId { get; set; }

    public UserInfo? UserInfo { get; set; }

    public string? Email { get; set; }

    public string? Memo { get; set; }

    public ICollection<ApplicationUserLoginHistory>? ApplicationUserLoginHistories { get; set; }

    public ICollection<ApplicationUserPoint>? ApplicationUserPoints { get; set; }

    /// <summary>ロール情報:UserManagerで引き合わされるためテーブルには存在させません</summary>
    public IList<string> Roles { get; set; } = new List<string>();

    /// <summary>
    /// 施設管理者および施設会員
    /// このリレーションは二つ以上は存在しない
    /// </summary>
    public ICollection<FacilityApplicationUser>? FacilityApplicationUsers { get; set; }

    /// <summary>
    /// アプリケーションユーザーお気に入りリレーション
    /// </summary>
    public ICollection<ApplicationUserFavorite>? ApplicationUserFavorites { get; set; }

    public static bool IsEmptyId(
        string id
    )
    {
        return string.IsNullOrEmpty(id);
    }

    public bool IsSameAuthUser(
        string code,
        string email
    )
    {
        var code1 = Code;
        var email1 = UserInfo?.EMail;

        if (string.IsNullOrEmpty(code))
        {
            return false;
        }

        if (string.IsNullOrEmpty(code1))
        {
            return false;
        }

        if (string.IsNullOrEmpty(email))
        {
            return false;
        }

        if (string.IsNullOrEmpty(email1))
        {
            return false;
        }

        return code1 == code && string.Equals(email1, email, StringComparison.CurrentCultureIgnoreCase);
    }
}
