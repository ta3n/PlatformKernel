namespace PlatformKernel.Entity.Auditing;

public interface IHasEraser<TUserKey>
{
    TUserKey DeletedBy { get; set; }
}
