using System.Text.Json;
using System.Text.Json.Serialization;

namespace SharedKernel.AuditLogging.Services;

internal static class AuditJsonSerializer
{
    internal static JsonSerializerOptions Options { get; } = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
}
