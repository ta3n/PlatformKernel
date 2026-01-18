using Liberty.Entity;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

/// <summary>
/// 入湯税年齢種別別設定
/// TODO 20190920 仕様書では宿が決めた「小学生＞高学年」「小学生＞低学年」というPersonAgeTypeをさらに細分化した項目に対して入湯税を個別設定できるようにしているが、実際は自治体によってさらに細かな分類があり実務で対応させればよいため、”入湯税に関するご案内”にその旨記載すればよい（牛島さん）
/// TODO 予約時にこの設定で保持（リレーション）されるため、「公開後の変更」は不可となり、削除は論理削除となる
/// TODO そのため「公開後の変更」を行った場合、自身を複製・新規作成し編集するようにする。元の派生であることがわかるように、派生前のIDを自身が保持する？
/// </summary>
public class SpaTaxData : EntityData
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

    /// <summary>
    /// 入湯税グループ・対象料金設定リレーション
    /// </summary>

    public ICollection<SpaTaxGroupSpaTaxData>? SpaTaxGroupSpaTaxData { get; set; }

    public ICollection<PersonAgeTypeSpaTaxData>? PersonAgeTypeSpaTaxData { get; set; }
}
