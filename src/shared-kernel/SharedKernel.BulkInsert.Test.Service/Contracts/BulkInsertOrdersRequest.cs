using SharedKernel.BulkInsert.Models;

namespace SharedKernel.BulkInsert.Test.Service.Contracts;

public sealed record BulkInsertOrdersRequest(
    int Count = 100,
    string Prefix = "order",
    string Source = "mini-api",
    PostgreSqlBulkInsertProvider Provider = PostgreSqlBulkInsertProvider.RepoDb,
    int? BatchSize = null,
    int? TimeoutSeconds = null
);
