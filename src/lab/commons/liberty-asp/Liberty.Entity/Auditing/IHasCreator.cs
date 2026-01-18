namespace Liberty.Entity.Auditing;

public interface IHasCreator<TUserKey>
{
    TUserKey CreatedBy { get; set; }
}
