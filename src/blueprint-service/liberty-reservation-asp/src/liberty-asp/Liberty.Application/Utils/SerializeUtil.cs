using Newtonsoft.Json;
using System.Text;

namespace Liberty.Application.Utils;

public class SerializeUtil
{
    /// <summary>
    /// Serialize
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static byte[] Serialize(object item)
    {
        var jsonString = JsonConvert.SerializeObject(item);

        return Encoding.UTF8.GetBytes(jsonString);
    }
    /// <summary>
    /// Deserialize
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static TEntity? Deserialize<TEntity>(byte[] value)
    {
        if (value == null)
        {
            return default;
        }
        var jsonString = Encoding.UTF8.GetString(value);
        return JsonConvert.DeserializeObject<TEntity>(jsonString);
    }
}
