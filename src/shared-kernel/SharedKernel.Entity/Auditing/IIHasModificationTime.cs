namespace SharedKernel.Entity.Auditing;

public interface IIHasModificationTime<TTimeType>
{
    TTimeType UpdatedAt { get; set; }
}
