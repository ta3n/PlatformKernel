using Testcontainers.RabbitMq;
using Xunit;

namespace SharedKernel.Rebus.Test.Integration;

public sealed class RabbitMqContainerFixture : IAsyncLifetime
{
    public const string ExternalConnectionStringEnvironmentVariable =
        "REBUS_TEST_RABBITMQ_CONNECTION_STRING";

    private readonly RabbitMqContainer? _rabbitMqContainer;
    private readonly bool _useExternalBroker;

    public RabbitMqContainerFixture()
    {
        var externalConnectionString = Environment.GetEnvironmentVariable(
            ExternalConnectionStringEnvironmentVariable
        );

        if (!string.IsNullOrWhiteSpace(externalConnectionString))
        {
            _useExternalBroker = true;
            ConnectionString = externalConnectionString;
            return;
        }

        _rabbitMqContainer = new RabbitMqBuilder()
            .WithImage("rabbitmq:3.13-management-alpine")
            .Build();
        ConnectionString = string.Empty;
    }

    public string ConnectionString { get; private set; }

    public async Task InitializeAsync()
    {
        if (_useExternalBroker)
        {
            return;
        }

        await _rabbitMqContainer!.StartAsync();
        ConnectionString = _rabbitMqContainer.GetConnectionString();
    }

    public async Task DisposeAsync()
    {
        if (_rabbitMqContainer is null)
        {
            return;
        }

        await _rabbitMqContainer.DisposeAsync();
    }
}
