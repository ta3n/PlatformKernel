namespace SharedKernel.BulkInsertOther.Abstractions;

public interface IBulkInsertPartitionRouter<in T>
{
    BulkInsertTableIdentifier ResolveTarget(
        T entity,
        BulkInsertTableIdentifier defaultTarget
    );
}
