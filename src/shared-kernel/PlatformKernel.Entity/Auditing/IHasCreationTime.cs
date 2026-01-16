namespace PlatformKernel.Entity.Auditing;

public interface IHasCreationTime<TTimeType>
{
    TTimeType CreatedAt { get; set; }
}
