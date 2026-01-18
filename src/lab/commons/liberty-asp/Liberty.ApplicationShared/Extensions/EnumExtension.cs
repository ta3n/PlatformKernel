using System.ComponentModel;

namespace Liberty.ApplicationShared.Extensions;

public static class EnumExtension
{
    public static string GetEnumDescriptions<T>(
        this T e
    ) where T : IConvertible
    {
        if (e is not Enum)
        {
            return string.Empty;
        }

        var field = e.GetType().GetField(e.ToString() ?? string.Empty);
        var customAttribute = field?.GetCustomAttributes(typeof(DescriptionAttribute), false);
        var description = customAttribute is { Length: > 0 }
            ? ((DescriptionAttribute)customAttribute[0]).Description
            : string.Empty;

        return description;
    }
}
