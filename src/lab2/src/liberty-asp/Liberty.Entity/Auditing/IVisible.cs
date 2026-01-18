namespace Liberty.Entity.Auditing;

public interface IVisible
{
    /// <summary>
    /// 公開・非公開
    /// </summary>
    bool IsVisible { get; set; }
}
