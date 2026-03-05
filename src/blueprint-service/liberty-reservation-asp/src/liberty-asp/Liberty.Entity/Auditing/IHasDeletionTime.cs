namespace Liberty.Entity.Auditing;

public interface IHasDeletionTime<TTimeType>
{
    TTimeType DeletedAt { get; set; }
}
