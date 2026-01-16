using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PlatformKernel.Service.Base.Application.Constants;

/// <summary>
/// Provides predefined JSON serializer settings optimized for specific use cases.
/// </summary>
public static class JsonSettings
{
    /// <summary>
    /// Provides preconfigured JSON serialization settings optimized for performance and readability.
    /// The settings include:
    /// - No extra whitespace in the output.
    /// - Null properties and default values are ignored.
    /// - Camel case naming convention is used for property names.
    /// - Object reference loops are ignored to prevent serialization errors.
    /// - Special handling for `NotSupportedException` to mark such errors as handled.
    /// </summary>
    public static readonly JsonSerializerSettings Optimized = new()
    {
        Formatting = Formatting.None, // No extra whitespace
        NullValueHandling = NullValueHandling.Ignore, // Ignore null properties
        DefaultValueHandling = DefaultValueHandling.Ignore, // Ignore default value properties
        ContractResolver = new DefaultContractResolver
        {
            IgnoreSerializableInterface = true,
            NamingStrategy = new CamelCaseNamingStrategy()
        }, // Use camelCase
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore, // Ignore object reference loops

        Error = (
            _,
            args
        ) =>
        {
            if (args.ErrorContext.Error is NotSupportedException)
            {
                args.ErrorContext.Handled = true;
            }
        }
    };
}
