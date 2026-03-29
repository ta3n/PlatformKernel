namespace SharedKernel.BulkInsert.Models;

public enum PostgreSqlBulkInsertProvider
{
    EfCore = 1,
    Dapper = 2,
    RepoDb = 3
}
