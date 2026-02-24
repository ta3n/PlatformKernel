using System.ComponentModel;

namespace SharedKernel.AppShared.Extensions;

/// <summary>
/// Provides extension methods for working with enumerations.
/// </summary>
public static class EnumExtension
{
    /// <summary>
    /// Retrieves the description of an enum value based on the Description attribute.
    /// </summary>
    /// <typeparam name="T">The enum type that implements IConvertible.</typeparam>
    /// <param name="e">The enum value for which to retrieve the description.</param>
    /// <returns>
    /// The description specified in the Description attribute of the enum value, or an empty string if the
    /// attribute is not present or the provided value is not a valid enum.
    /// </returns>
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
