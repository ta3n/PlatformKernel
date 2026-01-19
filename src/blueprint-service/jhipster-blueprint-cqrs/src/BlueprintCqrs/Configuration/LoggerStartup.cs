using JHipsterNet.Web.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

namespace BlueprintCqrs.Configuration;

public static class SerilogConfiguration
{
    private const string SerilogSection = "Serilog";
    private const string SyslogPort = "SyslogPort";
    private const string SyslogUrl = "SyslogUrl";
    private const string SyslogAppName = "SyslogAppName";

    /// <summary>
    /// Create application logger from configuration.
    /// </summary>
    /// <returns></returns>
    public static ILoggingBuilder AddSerilog(
        this ILoggingBuilder loggingBuilder,
        IConfiguration appConfiguration
    )
    {
        var port = 6514;

        // for logger configuration
        // https://github.com/serilog/serilog-settings-configuration
        if (appConfiguration.GetSection(SerilogSection)[SyslogPort] != null)
        {
            if (int.TryParse(appConfiguration.GetSection(SerilogSection)[SyslogPort], out var portFromConf))
            {
                port = portFromConf;
            }
        }

        var url = appConfiguration.GetSection(SerilogSection)[SyslogUrl] != null
            ? appConfiguration.GetSection(SerilogSection)[SyslogUrl]
            : "localhost";
        var appName = appConfiguration.GetSection(SerilogSection)[SyslogAppName] != null
            ? appConfiguration.GetSection(SerilogSection)[SyslogAppName]
            : "BlueprintCqrsApp";
        var loggerConfiguration = new LoggerConfiguration()
            .Enrich.With<LoggerNameEnricher>()
            .WriteTo.TcpSyslog(url, port, appName)
            .ReadFrom.Configuration(appConfiguration);

        Log.Logger = loggerConfiguration.CreateLogger();

        return loggingBuilder.AddSerilog(Log.Logger);
    }
}
