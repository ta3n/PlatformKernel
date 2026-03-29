namespace SharedKernel.BulkInsertPipeline.Test.Integration;

[CollectionDefinition(Name)]
public sealed class BulkInsertPostgresCollection : ICollectionFixture<BulkInsertPostgresFixture>
{
    public const string Name = "bulk-insert-other-postgres";
}
