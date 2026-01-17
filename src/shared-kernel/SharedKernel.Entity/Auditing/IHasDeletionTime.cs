namespace SharedKernel.Entity.Auditing;

public interface IHasDeletionTime<TTimeType>
{
    TTimeType DeletedAt { get; set; }
}
