using System.Text.Json.Nodes;
using SharedKernel.AuditLogging.Distributed.Contracts;

namespace SharedKernel.AuditLogging.Distributed.Services;

public static class AuditEventDiffService
{
    public static string CreateChangesJson(
        AuditEntityChanged auditEvent
    )
    {
        ArgumentNullException.ThrowIfNull(auditEvent);

        var before = ReadObject(auditEvent.BeforeJson);
        var after = ReadObject(auditEvent.AfterJson);
        var propertyNames = before.Select(static property => property.Key)
            .Concat(after.Select(static property => property.Key))
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal);
        var changes = new JsonObject();

        foreach (var propertyName in propertyNames)
        {
            before.TryGetPropertyValue(propertyName, out var oldValue);
            after.TryGetPropertyValue(propertyName, out var newValue);

            if (JsonNode.DeepEquals(oldValue, newValue))
            {
                continue;
            }

            changes[propertyName] = new JsonObject
            {
                ["old"] = CloneNode(oldValue),
                ["new"] = CloneNode(newValue)
            };
        }

        return changes.ToJsonString(AuditDistributedJsonSerializer.Options);
    }

    private static JsonObject ReadObject(
        string? json
    )
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return [];
        }

        var node = JsonNode.Parse(json);
        return node as JsonObject
            ?? throw new InvalidOperationException("Audit snapshot payload must be a JSON object.");
    }

    private static JsonNode? CloneNode(
        JsonNode? node
    )
    {
        return node is null
            ? null
            : JsonNode.Parse(node.ToJsonString(AuditDistributedJsonSerializer.Options));
    }
}
