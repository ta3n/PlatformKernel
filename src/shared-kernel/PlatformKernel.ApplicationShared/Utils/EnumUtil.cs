namespace PlatformKernel.ApplicationShared.Utils;

/// <summary>
/// Provides utility methods for working with enums.
/// </summary>
public static class EnumUtil
{
    /// <summary>
    /// Converts a string representation of the name of an enumeration to the equivalent enumeration object of type T.
    /// </summary>
    /// <typeparam name="T">The type of the enumeration.</typeparam>
    /// <param name="name">The name of the enumeration member to convert.</param>
    /// <returns>The enumeration object of type T corresponding to the provided name.</returns>
    /// <exception cref="ArgumentException">Thrown if the name is not a valid member of the enumeration type T.</exception>
    /// <exception cref="ArgumentNullException">Thrown if the name is null.</exception>
    public static T GetValue<T>(
        string name
    )
    {
        return (T)Enum.Parse(typeof(T), name);
    }

    /// <summary>
    /// Converts an integer value to its corresponding enumeration type.
    /// If the integer value is null, the method returns null.
    /// The method processes the flags of the enumeration if applicable.
    /// </summary>
    /// <typeparam name="T">The enumeration type to convert to, constrained to be an Enum.</typeparam>
    /// <param name="value">The nullable integer to be converted to the enumeration type.</param>
    /// <returns>
    /// The corresponding enumeration value of type <typeparamref name="T"/>
    /// or null if the input integer is null.
    /// </returns>
    public static T? IntToEnum<T>(
        int? value
    ) where T : struct, Enum
    {
        if (value == null)
        {
            return null;
        }

        var enumValue = default(T);

        foreach (var item in Enum.GetValues<T>())
        {
            var intValue = Convert.ToInt32(item);
            if ((value & intValue) == intValue)
            {
                enumValue = (T)Enum.ToObject(typeof(T), Convert.ToInt32(enumValue) | intValue);
            }
        }

        return enumValue;
    }

    /// <summary>
    /// Converts an enum value to its integer representation. This method takes a nullable enum value
    /// and calculates the combined integer value for all the specified enum flags in the value.
    /// </summary>
    /// <typeparam name="T">The type of the enum.</typeparam>
    /// <param name="enumValue">The nullable enum value to convert to an integer.</param>
    /// <returns>A nullable integer representing the combined value of the specified enum flags, or null if the input is null.</returns>
    public static int? EnumToInt<T>(
        T? enumValue
    ) where T : struct, Enum
    {
        if (enumValue == null)
        {
            return null;
        }

        return Enum.GetValues<T>()
            .Where(item => enumValue.Value.HasFlag(item))
            .Aggregate(
                0,
                (
                    current,
                    item
                ) => current | Convert.ToInt32(item)
            );
    }
}
