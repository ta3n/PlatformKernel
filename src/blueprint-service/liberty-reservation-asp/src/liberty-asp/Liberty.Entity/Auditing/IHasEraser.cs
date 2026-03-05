namespace Liberty.Entity.Auditing;

public interface IHasEraser<TUserKey>
{
    TUserKey DeletedBy { get; set; }
}
