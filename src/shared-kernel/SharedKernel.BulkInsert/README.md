# SharedKernel.BulkInsert

Plugin bulk insert cho PostgreSQL trong `shared-kernel`, gom 3 cách ghi dữ liệu dưới cùng một API:

- `PostgreSqlBulkInsertProvider.EfCore`: dùng `AddRange` + `SaveChanges` theo batch.
- `PostgreSqlBulkInsertProvider.Dapper`: dùng batched multi-values `INSERT`.
- `PostgreSqlBulkInsertProvider.RepoDb`: dùng `BinaryBulkInsertAsync`.

Tất cả provider đều đọc metadata bảng/cột từ EF Core model hiện có, nên vẫn tôn trọng các cấu hình như `ToTable`,
`HasColumnName`, schema và
naming convention.

## Registration

```csharp
builder.Services.AddPostgreSqlBulkInsert();
```

## Usage

```csharp
public sealed class OrderSeeder(
    IPostgreSqlBulkInsertService bulkInsertService,
    OrdersDbContext dbContext
)
{
    public Task<int> SeedAsync(
        IReadOnlyCollection<OrderEntity> orders,
        CancellationToken cancellationToken
    )
    {
        return bulkInsertService.BulkInsertAsync(
            dbContext,
            orders,
            PostgreSqlBulkInsertProvider.RepoDb,
            new PostgreSqlBulkInsertOptions
            {
                BatchSize = 1000,
                TimeoutSeconds = 60
            },
            cancellationToken
        );
    }
}
```

## Notes

- Chỉ hỗ trợ PostgreSQL thông qua `Npgsql`.
- Bảng và cột phải được map trong `DbContext` EF Core.
- Column sinh tự động bởi database như computed column hoặc value có `BeforeSaveBehavior = Ignore` sẽ bị bỏ qua khi dựng
  payload insert.
