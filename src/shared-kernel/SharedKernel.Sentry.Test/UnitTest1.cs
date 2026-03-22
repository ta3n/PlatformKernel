using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SharedKernel.Sentry;

namespace SharedKernel.Sentry.Test;

public class UnitTest1
{
    [Fact]
    public void AddSentryMonitoring_ReturnsBuilderWhenDisabled()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Configuration.AddInMemoryCollection(
        [
            new KeyValuePair<string, string?>("SentryMonitoring:Enabled", "false"),
            new KeyValuePair<string, string?>("SentryMonitoring:Dsn", "https://example@sentry.test/1"),
            new KeyValuePair<string, string?>("SentryMonitoring:Debug", "false"),
            new KeyValuePair<string, string?>("SentryMonitoring:TracesSampleRate", "0.1")
        ]);

        var result = builder.AddSentryMonitoring();

        Assert.Same(builder, result);
    }

    [Fact]
    public void UseSentryMonitoring_ReturnsSameApplicationWhenDisabled()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Configuration.AddInMemoryCollection(
        [
            new KeyValuePair<string, string?>("SentryMonitoring:Enabled", "false"),
            new KeyValuePair<string, string?>("SentryMonitoring:Dsn", "https://example@sentry.test/1"),
            new KeyValuePair<string, string?>("SentryMonitoring:Debug", "false"),
            new KeyValuePair<string, string?>("SentryMonitoring:TracesSampleRate", "0.1")
        ]);
        var app = builder.Build();

        var result = app.UseSentryMonitoring(app.Configuration);

        Assert.Same(app, result);
    }

    [Fact]
    public void SentryOptions_RecordPreservesValues()
    {
        var options = new SharedKernel.Sentry.Options.SentryOptions(true, "dsn", Debug: false, TracesSampleRate: 0.5);

        Assert.True(options.Enabled);
        Assert.Equal("dsn", options.Dsn);
        Assert.False(options.Debug);
        Assert.Equal(0.5, options.TracesSampleRate);
    }
}
