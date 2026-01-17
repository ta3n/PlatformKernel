using Serilog.Core;
using Serilog.Events;

namespace SharedKernel.Serilog;

/// <summary>
/// LoggerNameEnricher is a custom implementation of the <see cref="ILogEventEnricher"/> interface.
/// It enriches log events by adding a "LoggerName" property, which is derived from the fully qualified
/// class name of the source context.
/// </summary>
/// <remarks>
/// This enricher is particularly useful in logging scenarios where class name or source context information
/// is required to better understand the origin of log messages in a structured and readable format.
/// </remarks>
public class LoggerNameEnricher : ILogEventEnricher
{
    /// <summary>
    /// Represents the name of the Serilog property used to indicate the source context of the log event.
    /// </summary>
    /// <remarks>
    /// This constant is primarily used to retrieve the name of the logger or the fully qualified class name
    /// in the logging context. It aids in processing log events by identifying the originating class or context.
    /// </remarks>
    private const string SourceContextPropertyName = "SourceContext";

    /// <summary>
    /// Enriches a log event by adding a "LoggerName" property containing an abbreviated
    /// representation of the source context class name.
    /// </summary>
    /// <param name="logEvent">
    /// The log event to enrich with the "LoggerName" property.
    /// </param>
    /// <param name="propertyFactory">
    /// The property factory used to create new properties for the log event.
    /// </param>
    public void Enrich(
        LogEvent logEvent,
        ILogEventPropertyFactory propertyFactory
    )
    {
        var fullQualifiedClassName = string.Empty;
        if (logEvent.Properties.TryGetValue(SourceContextPropertyName, out var logEventProperty))
        {
            fullQualifiedClassName = logEventProperty.ToString().Trim('"');
        }

        logEvent.AddPropertyIfAbsent(
            propertyFactory.CreateProperty(
                "LoggerName",
                GetAbbreviatedClassName(fullQualifiedClassName, 39)
            )
        );
    }

    /// <summary>
    /// Generates an abbreviated class name based on the full-qualified class name.
    /// </summary>
    /// <param name="fullQualifiedClassName">
    /// The full name of the class, including namespace, to be abbreviated.
    /// </param>
    /// <param name="length">
    /// The maximum length of the abbreviated class name. Defaults to 0, which does not apply a length restriction.
    /// </param>
    /// <returns>
    /// Returns the abbreviated class name as a string. If the input is null or empty, returns an empty string.
    /// </returns>
    private static string GetAbbreviatedClassName(
        string fullQualifiedClassName,
        int length = 0
    )
    {
        if (string.IsNullOrEmpty(fullQualifiedClassName))
        {
            return string.Empty;
        }

        var source = fullQualifiedClassName.Split('.');
        var abbreviatedClassName = source[^1];
        if (length == 0)
        {
            return abbreviatedClassName;
        }

        var strArray = new string[source.Length];
        strArray[source.Length - 1] = abbreviatedClassName;
        var num = length - abbreviatedClassName.Length - 1;
        for (var index = source.Length - 2; index >= 0; --index)
        {
            var str = source[index];
            num = num - str.Length - 1;
            strArray[index] = num <= 0 ? str.Substring(0, 1) : str;
        }

        return string.Join(".", strArray);
    }
}
