using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Liberty.Application.Utils
{
    public class EncryptUtil
    {
        public static string MD5(string source)
        {
            return GetHashString<MD5CryptoServiceProvider>(source);
        }

        public static string GetHashString<T>(string text) where T : HashAlgorithm, new()
        {
            // 文字列をバイト型配列に変換する
            byte[] data = Encoding.UTF8.GetBytes(text);

            // ハッシュアルゴリズム生成
            var algorithm = new T();

            // ハッシュ値を計算する
            byte[] bs = algorithm.ComputeHash(data);

            // リソースを解放する
            algorithm.Clear();

            // バイト型配列を16進数文字列に変換
            var result = new StringBuilder();
            foreach (byte b in bs)
            {
                result.Append(ConvertUtil.ToString(b, "X2"));
            }
            return result.ToString().ToLower(new System.Globalization.CultureInfo("en-US"));
        }
    }
}
