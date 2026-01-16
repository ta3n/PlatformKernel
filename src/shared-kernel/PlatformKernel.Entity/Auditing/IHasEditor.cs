namespace PlatformKernel.Entity.Auditing;

public interface IHasEditor<TUserKey>
{
    TUserKey UpdatedBy { get; set; }
}
