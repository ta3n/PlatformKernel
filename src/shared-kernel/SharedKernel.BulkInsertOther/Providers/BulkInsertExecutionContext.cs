using SharedKernel.BulkInsertOther.Mapping;

namespace SharedKernel.BulkInsertOther.Providers;

internal readonly record struct BulkInsertExecutionContext<T>(
    BulkInsertEntityDescriptor<T> Descriptor,
    BulkInsertTableIdentifier Target
);
