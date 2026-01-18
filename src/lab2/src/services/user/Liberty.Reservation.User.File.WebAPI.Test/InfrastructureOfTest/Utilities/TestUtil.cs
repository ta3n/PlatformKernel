using System.Text;
using Newtonsoft.Json;

namespace Liberty.Reservation.User.File.WebAPI.Test.InfrastructureOfTest.Utilities;

public static class TestUtil
{
    private static readonly Random Random = new();

    public const string DefaultLanguageCode = "ja";
    public const string DefaultAcceptLanguage = "ja-JP";

    public static HttpContent ToJsonContent(
        object model
    )
    {
        return ToJsonContent(model, Encoding.UTF8);
    }

    private static HttpContent ToJsonContent(
        object model,
        Encoding encoding
    )
    {
        return new StringContent(
            JsonConvert.SerializeObject(model),
            encoding,
            "application/json"
        );
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

    public static bool IsUseTestSqlite()
    {
        var envUseTestSqlite = Environment.GetEnvironmentVariable("UseTestSqlite");
        return !string.IsNullOrEmpty(envUseTestSqlite);
    }
}
