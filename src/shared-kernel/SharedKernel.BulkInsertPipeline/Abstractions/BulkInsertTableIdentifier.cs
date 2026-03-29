namespace SharedKernel.BulkInsertPipeline.Abstractions;

public readonly record struct BulkInsertTableIdentifier(
    string Schema,
    string Table
)
{
    public string QualifiedName => $"{QuoteIdentifier(Schema)}.{QuoteIdentifier(Table)}";

    public static string QuoteIdentifier(
        string identifier
    )
    {
        return $"\"{identifier.Replace("\"", "\"\"", StringComparison.Ordinal)}\"";
    }
}
