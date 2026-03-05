using Liberty.Entity;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using System.ComponentModel.DataAnnotations.Schema;
using Liberty.Entity.ValueObjects;

namespace Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

/// <summary>
/// カテゴリ
/// </summary>
public class Category : EntityData
{
    /// <summary>
    /// カテゴリ種別
    /// </summary>
    public CategoryTypes CategoryType { get; set; }

    /// <summary>
    /// カテゴリ名称
    /// </summary>
    public MultilingualText? Name { get; set; }

    /// <summary>
    /// カテゴリ説明
    /// </summary>
    public MultilingualText? Description { get; set; }

    public bool IsMaster { get; set; }

    [ForeignKey("Parent")]
    public long? ParentId { set; get; }

    public Category? Parent { set; get; }

    public ICollection<Category>? Children { set; get; }

    /// <summary>
    /// 施設リレーション
    /// </summary>
    public ICollection<FacilityCategory>? FacilityCategories { get; set; }

    /// <summary>
    /// 部屋-カテゴリリレーション
    /// </summary>
    public ICollection<RoomGroupCategory>? RoomGroupCategories { get; set; }

    /// <summary>
    /// プラン-カテゴリリレーション
    /// </summary>
    public ICollection<PlanCategory>? PlanCategories { get; set; }

    /// <summary>
    /// オプションアイテムカテゴリリレーション
    /// </summary>
    public ICollection<OptionItemCategory>? OptionItemCategories { get; set; }

    public ICollection<FileCategory>? FileCategories { get; set; }
}
