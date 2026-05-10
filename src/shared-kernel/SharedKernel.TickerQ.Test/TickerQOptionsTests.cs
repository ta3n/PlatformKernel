using FluentAssertions;
using SharedKernel.TickerQ.Options;
using Xunit;

namespace SharedKernel.TickerQ.Test;

public sealed class TickerQOptionsTests
{
    [Fact]
    public void TickerQStorageOptions_ShouldHaveDefaultValues()
    {
        // Arrange & Act
        var options = new TickerQStorageOptions();

        // Assert
        options.Provider.Should().Be(TickerQStorageProviders.EntityFramework);
        options.ConnectionStringName.Should().Be("TickerQConnection");
        options.CancelMissedTickersOnRestart.Should().BeTrue();
        options.UseModelCustomizerForMigrations.Should().BeTrue();
        options.Redis.Should().NotBeNull();
    }

    [Fact]
    public void TickerQServerOptions_ShouldHaveDefaultValues()
    {
        // Arrange & Act
        var options = new TickerQServerOptions();

        // Assert
        options.Enabled.Should().BeTrue();
        options.MaxConcurrency.Should().Be(4);
        options.DefaultRetries.Should().Be(3);
        options.DefaultRetryIntervalsInSeconds.Should().BeEquivalentTo(new[] { 60, 120, 300 });
        options.BootstrapLockName.Should().Be("tickerq:bootstrap");
        options.BootstrapLockTimeoutInSeconds.Should().Be(180);
    }

    [Fact]
    public void TickerQDashboardOptions_ShouldHaveDefaultValues()
    {
        // Arrange & Act
        var options = new TickerQDashboardOptions();

        // Assert
        options.Enabled.Should().BeFalse();
        options.DashboardUrl.Should().Be("tickerq-dashboard");
        options.EnableBasicAuth.Should().BeTrue();
        options.IsReadOnly.Should().BeFalse();
    }

    [Fact]
    public void TickerQRedisStorageOptions_ShouldHaveDefaultPrefix()
    {
        // Arrange & Act
        var options = new TickerQRedisStorageOptions();

        // Assert
        options.Prefix.Should().Be("tickerq:");
        options.ConnectionString.Should().BeNull();
        options.Database.Should().BeNull();
    }
}
