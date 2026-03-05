namespace Liberty.Entity.Auditing;

public interface IEnable
{
    /// <summary>
    /// 有効か？
    /// </summary>
    bool IsEnabled { get; set; }
}
