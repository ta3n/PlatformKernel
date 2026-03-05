using System.Security.Cryptography;
using System.Text;

namespace Liberty.ApplicationShared.Utils;

public static class EncryptUtil
{
    public static string Sha256(
        string? source
    )
    {
        if (source is null)
        {
            return string.Empty;
        }

        var inputBytes = Encoding.ASCII.GetBytes(source);
        var hashBytes = SHA256.HashData(inputBytes);

        // Convert the byte array to hexadecimal string
        var sb = new StringBuilder();
        foreach (var t in hashBytes)
        {
            sb.Append(t.ToString("X2"));
        }

        return sb.ToString();
    }

    public static string GetHashString<T>(
        string text
    ) where T : HashAlgorithm, new()
    {
        // 文字列をバイト型配列に変換する
        var data = Encoding.UTF8.GetBytes(text);

        // ハッシュアルゴリズム生成
        var algorithm = new T();

        // ハッシュ値を計算する
        var bs = algorithm.ComputeHash(data);

        // リソースを解放する
        algorithm.Clear();

        // バイト型配列を16進数文字列に変換
        var result = new StringBuilder();
        foreach (var b in bs)
        {
            result.Append(ConvertUtil.ToString(b, "X2"));
        }

        return result.ToString().ToLower(new System.Globalization.CultureInfo("en-US"));
    }
}
