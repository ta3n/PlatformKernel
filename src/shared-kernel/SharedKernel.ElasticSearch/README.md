# SharedKernel.ElasticSearch

Plugin ElasticSearch cho `shared-kernel`, tập trung vào 3 lớp:

- `ElasticSearchOptions`: bind cấu hình từ `appsettings`.
- `ElasticsearchClient`: official client đã được cấu hình sẵn qua DI.
- `IElasticSearchService`: wrapper cho các thao tác thông dụng như `Ping`, `EnsureIndex`, `Index`, `Get`, `Search`, `Upsert`, `Delete`,
  `BulkIndex`, `Refresh`.

## Configuration

```json
{
  "ElasticSearch": {
    "Enabled": true,
    "Endpoint": "https://localhost:9200",
    "Username": "elastic",
    "Password": "changeme",
    "DefaultIndex": "platform-default",
    "CertificateFingerprint": "YOUR_SHA256_FINGERPRINT",
    "EnableDebugMode": false,
    "PrettyJson": false,
    "DisableDirectStreaming": false,
    "EnableHttpCompression": true,
    "ThrowExceptions": false,
    "DisablePing": false,
    "MaximumRetries": 2,
    "ConnectionLimit": 80,
    "RequestTimeoutSeconds": 30,
    "PingTimeoutSeconds": 5,
    "DeadTimeoutSeconds": 60,
    "MaxDeadTimeoutSeconds": 180,
    "MaxRetryTimeoutSeconds": 60,
    "SniffOnStartup": false,
    "SniffOnConnectionFault": false,
    "TcpKeepAliveTimeSeconds": 30,
    "TcpKeepAliveIntervalSeconds": 10,
    "Indexes": {
      "ProductSearchDocument": "products-v1"
    },
    "HealthCheck": {
      "Enabled": true,
      "Name": "elasticsearch",
      "FailureStatus": "Unhealthy",
      "Tags": [ "ready" ]
    }
  }
}
```

Elastic Cloud cũng được hỗ trợ:

```json
{
  "ElasticSearch": {
    "Enabled": true,
    "CloudId": "your-cloud-id",
    "ApiKey": "your-api-key",
    "DefaultIndex": "platform-default"
  }
}
```

## Registration

```csharp
builder.Services.AddElasticSearch(builder.Configuration);
```

## Usage

```csharp
public sealed class ProductSearchRepository(
    IElasticSearchService elasticSearchService
)
{
    public Task IndexAsync(ProductSearchDocument document, CancellationToken cancellationToken)
        => elasticSearchService.IndexAsync(document, cancellationToken: cancellationToken);

    public Task<SearchResponse<ProductSearchDocument>> SearchAsync(string keyword, CancellationToken cancellationToken)
        => elasticSearchService.SearchAsync<ProductSearchDocument>(
            search => search
                .From(0)
                .Size(20)
                .Query(query => query
                    .MultiMatch(match => match
                        .Fields(["name", "sku", "description"])
                        .Query(keyword)
                    )
                ),
            cancellationToken: cancellationToken
        );
}
```

Nếu cần API nâng cao hơn, lấy trực tiếp official client qua `IElasticSearchService.Client` hoặc `IElasticSearchClientFactory`.
