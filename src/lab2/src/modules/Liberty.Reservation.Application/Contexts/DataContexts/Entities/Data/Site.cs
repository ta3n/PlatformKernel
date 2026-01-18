using Liberty.Entity;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

/// <summary>
/// サイト
/// </summary>
public class Site : EntityData
{
    /// <summary>
    /// サイト名
    /// </summary>
    public MultilingualText? Name { get; set; }

    public string? ShortName { get; set; }

    public string? PrefixName { get; set; }

    /// <summary>
    /// サイトURL
    /// </summary>
    public string? Url { get; set; }

    public string? Description { get; set; }
    public string? Memo { get; set; }

    /// <summary>
    /// サイト独自のポイント設定を使うか？
    /// </summary>
    public bool UseSitePoint { get; set; }

    public bool IsMaster { get; set; }

    /// <summary>
    /// タグ
    /// </summary>
    public string? Tag { get; set; }

    /// <summary>
    /// 情報JSON
    /// </summary>

    public SiteMeta? Meta { get; set; }

    /// <summary>
    /// 掲載先サイトリレーション
    /// </summary>
    public ICollection<FacilitySite>? FacilitySites { get; set; }

    /// <summary>
    /// プラン サイト　リレーション
    /// </summary>
    public ICollection<PlanSite>? PlanSites { get; set; }

    public ICollection<PlanRoomGroupSite>? PlanRoomGroupSites { get; set; }

    public ICollection<RoomGroupSite>? RoomGroupSites { get; set; }

    /// <summary>
    /// プランサイト部屋日付リレーション
    /// </summary>
    public ICollection<PlanRoomGroupSiteAppDate>? PlanRoomGroupSiteAppDates { get; set; }

    /// <summary>
    /// プラン部屋サイト日別料金リレーション
    /// </summary>
    public ICollection<PlanRoomGroupSiteAppDatePriceData>? PlanRoomSiteAppDatePriceData { get; set; }

    public ICollection<PlanRoomGroupSitePriceData>? PlanRoomGroupSitePriceData { get; set; }

    /// <summary>
    /// サイト部屋日付リレーション
    /// </summary>
    public ICollection<RoomGroupSiteAppDate>? RoomGroupSiteAppDates { get; set; }

    /// <summary>
    /// プラン部屋サイト割引リレーション
    /// </summary>
    public ICollection<PlanRoomGroupSiteDiscountData>? PlanRoomGroupSiteDiscountData { get; set; }

    public ICollection<PlanRoomGroupSiteAppDateTypePriceData>? PlanRoomGroupSiteAppDateTypePriceData { get; set; }

    /// <summary>
    /// 施設毎に年齢種別に対する設定をサイト毎にプランの部屋でどのように販売するかの設定
    /// </summary>
    public ICollection<PlanRoomGroupSitePersonAgeType>? PlanRoomGroupSitePersonAgeTypes { get; set; }

    public ICollection<RoomGroupSiteAppDatePriceData>? RoomGroupSiteAppDatePriceData { get; set; }

    public ICollection<RoomGroupSiteAppDateTypePriceData>? RoomGroupSiteAppDateTypePriceData { get; set; }

    public ICollection<SitePointRate>? SitePointRates { get; set; }
}
