using Testcontainers.PostgreSql;

namespace SharedKernel.Hangfire.Test.Integration;

public sealed class HangfirePostgreSqlContainerFixture : IAsyncLifetime
{
    public const string ExternalConnectionStringEnvironmentVariable =
        "HANGFIRE_TEST_POSTGRES_CONNECTION";

    private readonly PostgreSqlContainer? _postgreSqlContainer;

    public HangfirePostgreSqlContainerFixture()
    {
        var externalConnectionString = Environment.GetEnvironmentVariable(
            ExternalConnectionStringEnvironmentVariable
        );

        if (!string.IsNullOrWhiteSpace(externalConnectionString))
        {
            ConnectionString = externalConnectionString;
            return;
        }

        _postgreSqlContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .WithDatabase("shared_kernel_hangfire_test")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();
        ConnectionString = string.Empty;
    }

    public string ConnectionString { get; private set; }

    public async Task InitializeAsync()
    {
        if (_postgreSqlContainer is null)
        {
            return;
        }

        await _postgreSqlContainer.StartAsync();
        ConnectionString = _postgreSqlContainer.GetConnectionString();
    }

    public async Task DisposeAsync()
    {
        if (_postgreSqlContainer is not null)
        {
            await _postgreSqlContainer.DisposeAsync();
        }
    }
}
