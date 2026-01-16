namespace PlatformKernel.Entity.Auditing;

public interface IHasCreator<TUserKey>
{
    TUserKey CreatedBy { get; set; }
}
