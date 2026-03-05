using System.Reflection;
using System.Text.Json.Serialization;

namespace Liberty.GmoPaymentGateway.Utils;

/// <summary>
/// Provides utility methods for data parsing and conversion in applications.
/// </summary>
public static class HelperUtil
{
    /// <summary>
    /// Converts a response string into an object of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the object to convert to. The type must have a parameterless constructor.</typeparam>
    /// <param name="response">The string response to be converted into the specified type.</param>
    /// <returns>An object of type <typeparamref name="T"/> populated with the data from the response string.</returns>
    public static T Convert<T>(
        string response
    ) where T : new()
    {
        var dic = new Dictionary<string, string>();

        foreach (var keyValue in response.Split("&"))
        {
            var keyValueAry = keyValue.Split("=");
            var key = keyValueAry[0];
            var val = keyValueAry[1];
            dic.Add(key, val);
        }

        var result = new T();
        var objectType = typeof(T);
        var objectProperties = objectType.GetProperties();
        foreach (var key in dic.Keys)
        {
            var obj = dic[key];

            var propertyInfo = Array.Find(
                    objectProperties,
                    p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == key
                )
                ?? objectType.GetProperty(key);

            propertyInfo?.SetValue(result, obj);
        }

        return result;
    }
}
