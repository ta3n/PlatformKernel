using SharedKernel.BulkInsertPipeline.Mapping;

namespace SharedKernel.BulkInsertPipeline.Providers;

internal readonly record struct BulkInsertExecutionContext<T>(
    BulkInsertEntityDescriptor<T> Descriptor,
    BulkInsertTableIdentifier Target
);
