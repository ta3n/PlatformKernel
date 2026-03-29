namespace SharedKernel.BulkInsertOther.Abstractions;

public enum BulkInsertProviderType
{
    NpgsqlBinaryCopy = 0,
    Dapper = 1,
    RepoDb = 2,
    EfCore = 3
}
