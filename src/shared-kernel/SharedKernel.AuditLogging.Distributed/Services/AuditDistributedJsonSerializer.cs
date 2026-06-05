using System.Text.Json;
using System.Text.Json.Serialization;

namespace SharedKernel.AuditLogging.Distributed.Services;

internal static class AuditDistributedJsonSerializer
{
    internal static JsonSerializerOptions Options { get; } = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
}
