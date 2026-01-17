namespace SharedKernel.Entity.Auditing;

public interface IHasEditor<TUserKey>
{
    TUserKey UpdatedBy { get; set; }
}
