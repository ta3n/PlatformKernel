using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Liberty.Reservation.Application.Utils;

/// <summary>
/// Utility class for comparing objects, sequences, and generating differences.
/// </summary>
public static class CompareUtil
{
    /// <summary>
    /// Gets the differences between two objects of the same type by comparing their properties.
    /// </summary>
    /// <typeparam name="T">The type of the objects to compare.</typeparam>
    /// <param name="oldObj">The old object to compare. Can be <c>null</c>.</param>
    /// <param name="newObj">The new object to compare. Can be <c>null</c>.</param>
    /// <param name="ignoreProperties">
    /// An array of property names to ignore during the comparison.
    /// </param>
    /// <returns>
    /// A list of tuples where each tuple contains:
    /// <list type="bullet">
    /// <item><description>The name of the property that differs.</description></item>
    /// <item><description>The value of the property in the old object.</description></item>
    /// <item><description>The value of the property in the new object.</description></item>
    /// </list>
    /// </returns>
    /// <example>
    /// <code>
    /// var oldObj = new { Name = "Alice", Age = 30 };
    /// var newObj = new { Name = "Alice", Age = 31 };
    /// var differences = CompareUtil.GetDifferences(oldObj, newObj);
    /// // Result: [("Age", 30, 31)]
    /// </code>
    /// </example>
    public static List<(string? PropName, object? BeforeValue, object? AfterValue)> GetDifferences<T>(
        T? oldObj,
        T? newObj,
        params string[] ignoreProperties
    )
        where T : class
    {
        return (
            from prop in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            where prop.CanRead && !ignoreProperties.Contains(prop.Name)
            let oldValue = oldObj != null ? prop.GetValue(oldObj) : null
            let newValue = newObj != null ? prop.GetValue(newObj) : null
            where !Equals(oldValue, newValue)
            select (prop.Name, oldValue, newValue)
        ).ToList();
    }

    /// <summary>
    /// Compares two objects by serializing them to JSON and checking if the resulting JSON strings are equal.
    /// </summary>
    /// <typeparam name="T">The type of the objects to compare.</typeparam>
    /// <param name="obj1">The first object to compare.</param>
    /// <param name="obj2">The second object to compare.</param>
    /// <returns>
    /// <c>true</c> if the serialized JSON strings are equal; otherwise, <c>false</c>.
    /// </returns>
    public static bool AreEqualByJson<T>(
        T obj1,
        T obj2
    )
    {
        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true
        };

        var json1 = JsonSerializer.Serialize(obj1, jsonOptions);
        var json2 = JsonSerializer.Serialize(obj2, jsonOptions);

        return json1 == json2;
    }

    /// <summary>
    /// Compares two sequences for equality using a custom comparer function.
    /// </summary>
    /// <typeparam name="T">The type of elements in the sequences.</typeparam>
    /// <param name="a">The first sequence to compare.</param>
    /// <param name="b">The second sequence to compare.</param>
    /// <param name="comparer">A function to compare individual elements.</param>
    /// <returns>
    /// <c>true</c> if the sequences are equal; otherwise, <c>false</c>.
    /// </returns>
    public static bool SequenceEqual<T>(
        IEnumerable<T>? a,
        IEnumerable<T>? b,
        Func<T, T, bool> comparer
    )
    {
        if (a is null && b is null)
        {
            return true;
        }

        if (a is null || b is null)
        {
            return false;
        }

        var aList = a.ToList();
        var bList = b.ToList();

        if (aList.Count != bList.Count)
        {
            return false;
        }

        return !aList.Where(
                (
                    t,
                    i
                ) => !comparer(t, bList[i])
            )
            .Any();
    }

    /// <summary>
    /// Computes the differences between two lists by comparing their elements using a key selector and a custom comparer.
    /// </summary>
    /// <typeparam name="T">The type of elements in the lists.</typeparam>
    /// <typeparam name="TKey">The type of the key used to identify elements.</typeparam>
    /// <param name="oldList">The old list to compare.</param>
    /// <param name="newList">The new list to compare.</param>
    /// <param name="keySelector">A function to extract the key from an element.</param>
    /// <param name="comparer">A function to compare individual elements.</param>
    /// <returns>
    /// A list of <see cref="ChangeLog{T}"/> representing the differences between the two lists.
    /// </returns>
    public static List<ChangeLog<T>> Diff<T, TKey>(
        List<T> oldList,
        List<T> newList,
        Func<T, TKey> keySelector,
        Func<T, T, bool> comparer
    )
        where TKey : notnull
    {
        var oldDict = oldList.GroupBy(keySelector).ToDictionary(g => g.Key, g => g.First());
        var newDict = newList.GroupBy(keySelector).ToDictionary(g => g.Key, g => g.First());

        var result = new List<ChangeLog<T>>();

        foreach (var newEntry in newDict)
        {
            if (!oldDict.TryGetValue(newEntry.Key, out var oldValue))
            {
                result.Add(new ChangeLog<T>(default, newEntry.Value, ChangeType.Added));
            }
            else if (!comparer(oldValue, newEntry.Value))
            {
                result.Add(new ChangeLog<T>(oldValue, newEntry.Value, ChangeType.Modified));
            }
        }

        result.AddRange(
            from oldEntry in oldDict
            where !newDict.ContainsKey(oldEntry.Key)
            select new ChangeLog<T>(oldEntry.Value, default, ChangeType.Removed)
        );

        return result;
    }
}

/// <summary>
/// Represents the type of change in a comparison.
/// </summary>
public enum ChangeType
{
    /// <summary>
    /// Indicates that an item was added.
    /// </summary>
    Added,

    /// <summary>
    /// Indicates that an item was removed.
    /// </summary>
    Removed,

    /// <summary>
    /// Indicates that an item was modified.
    /// </summary>
    Modified
}

/// <summary>
/// Represents a log of changes between two objects.
/// </summary>
/// <typeparam name="T">The type of the objects being compared.</typeparam>
/// <param name="OldValue">The old value of the object.</param>
/// <param name="NewValue">The new value of the object.</param>
/// <param name="Type">The type of change.</param>
public record ChangeLog<T>(
    T? OldValue,
    T? NewValue,
    ChangeType Type
);
