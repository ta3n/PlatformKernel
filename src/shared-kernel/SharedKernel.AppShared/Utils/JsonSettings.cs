using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SharedKernel.AppShared.Utils;

/// <summary>
/// Provides predefined JSON serializer settings optimized for specific use cases.
/// </summary>
public static class JsonSettings
{
    /// <summary>
    /// Provides preconfigured JSON serialization options optimized for performance and readability using System.Text.Json.
    /// The settings include:
    /// - No extra whitespace in the output.
    /// - Null properties and default values are ignored.
    /// - Camel case naming convention is used for property names.
    /// - Object reference loops are ignored to prevent serialization errors.
    /// </summary>
    public static readonly JsonSerializerOptions OptimizedSystemTextJson = new()
    {
        WriteIndented = false, // No extra whitespace
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, // Ignore null properties
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // Use camelCase
        ReferenceHandler = ReferenceHandler.IgnoreCycles, // Ignore object reference loops
        PropertyNameCaseInsensitive = true, // Case-insensitive property name matching
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping // Less aggressive escaping
    };
}
