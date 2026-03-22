using System.Collections.Concurrent;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Serilog.Core;
using Serilog.Events;
using SharedKernel.Serilog;

namespace SharedKernel.Serilog.Test;

public class UnitTest1
{
    [Fact]
    public void LoggerNameEnricher_AddsAbbreviatedLoggerName()
    {
        var logEvent = new LogEvent(
            DateTimeOffset.UtcNow,
            LogEventLevel.Information,
            null,
            new MessageTemplate(string.Empty, []),
            [new LogEventProperty("SourceContext", new ScalarValue("Platform.Kernel.Services.DemoHandler"))]);
        var propertyFactory = new TestLogEventPropertyFactory();
        var enricher = new LoggerNameEnricher();

        enricher.Enrich(logEvent, propertyFactory);

        Assert.Equal("\"Platform.Kernel.Services.DemoHandler\"", logEvent.Properties["LoggerName"].ToString());
    }

    [Fact]
    public void AddSerilogLogging_WhenDisabled_DoesNotRegisterLogQueue()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Configuration.AddInMemoryCollection(
        [
            new KeyValuePair<string, string?>("SerilogLogging:Enabled", "false"),
            new KeyValuePair<string, string?>("SerilogLogging:WriteToConsole", "false"),
            new KeyValuePair<string, string?>("SerilogLogging:WriteToFile", "false")
        ]);

        builder.AddSerilogLogging("Kernel");

        Assert.DoesNotContain(builder.Services, descriptor => descriptor.ServiceType == typeof(ConcurrentQueue<string>));
    }

    [Fact]
    public void AddSerilogLogging_WhenEnabled_RegistersLogProcessorDependencies()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Configuration.AddInMemoryCollection(
        [
            new KeyValuePair<string, string?>("SerilogLogging:Enabled", "true"),
            new KeyValuePair<string, string?>("SerilogLogging:WriteToConsole", "false"),
            new KeyValuePair<string, string?>("SerilogLogging:WriteToFile", "false")
        ]);

        builder.AddSerilogLogging("Kernel");

        Assert.Contains(builder.Services, descriptor => descriptor.ServiceType == typeof(ConcurrentQueue<string>));
    }

    private sealed class TestLogEventPropertyFactory : ILogEventPropertyFactory
    {
        public LogEventProperty CreateProperty(string name, object? value, bool destructureObjects = false)
        {
            return new LogEventProperty(name, new ScalarValue(value));
        }
    }
}
