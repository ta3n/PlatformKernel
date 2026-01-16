namespace PlatformKernel.Entity;

public interface ILogicalDelete
{
    /// <summary>
    /// 論理削除フラグ
    /// </summary>
    bool IsDeleted { get; set; }
}
