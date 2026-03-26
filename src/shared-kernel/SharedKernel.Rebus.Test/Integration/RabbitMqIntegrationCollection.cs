using Xunit;

namespace SharedKernel.Rebus.Test.Integration;

[CollectionDefinition(CollectionName)]
public sealed class RabbitMqIntegrationCollection : ICollectionFixture<RabbitMqContainerFixture>
{
    public const string CollectionName = "RabbitMqIntegration";
}
