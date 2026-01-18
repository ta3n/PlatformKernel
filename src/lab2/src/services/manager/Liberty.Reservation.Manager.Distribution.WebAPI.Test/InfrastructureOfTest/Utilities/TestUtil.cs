using System.Text;
using System.Xml.Serialization;
using System.Xml;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Test.InfrastructureOfTest.Utilities;

public static class TestUtil
{
    private static readonly Random Random = new();

    public const string DefaultLanguageCode = "ja";
    public const string DefaultAcceptLanguage = "ja-JP";

    public static HttpContent ToXmlContent(
        object model
    )
    {
        var xmlSerializer = new XmlSerializer(model.GetType());

        var namespaces = new XmlSerializerNamespaces();
        namespaces.Add(string.Empty, string.Empty);

        var settings = new XmlWriterSettings
        {
            Encoding = new UTF8Encoding(false),
            Indent = true,
            OmitXmlDeclaration = false
        };

        using var memoryStream = new MemoryStream();
        using (var writer = XmlWriter.Create(memoryStream, settings))
        {
            xmlSerializer.Serialize(writer, model, namespaces);
        }

        var xml = Encoding.UTF8.GetString(memoryStream.ToArray());
        return new StringContent(xml, Encoding.UTF8, "application/xml");
    }

    public static string RandomAlphabetic(
        int length
    )
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        return new string(
            [.. Enumerable.Repeat(chars, length).Select(s => s[Random.Next(s.Length)])]
        );
    }

    public static string RandomNumeric(
        int length
    )
    {
        const string chars = "0123456789";
        return new string(
            [.. Enumerable.Repeat(chars, length).Select(s => s[Random.Next(s.Length)])]
        );
    }

    public static int RandomInt(
        int min,
        int max
    )
    {
        return Random.Next(min, max);
    }

    public static bool IsUseTestSqlite()
    {
        var envUseTestSqlite = Environment.GetEnvironmentVariable("UseTestSqlite");
        return !string.IsNullOrEmpty(envUseTestSqlite);
    }
}
