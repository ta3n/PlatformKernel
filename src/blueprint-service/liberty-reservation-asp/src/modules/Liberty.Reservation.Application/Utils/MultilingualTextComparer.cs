using Liberty.Entity.ValueObjects;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Liberty.Reservation.Application.Utils;

/// <summary>
/// Provides functionality for comparing instances of <see cref="MultilingualText"/>.
/// </summary>
/// <remarks>
/// This class is a custom implementation of <see cref="ValueComparer{T}"/> for the
/// <see cref="MultilingualText"/> type, allowing comparisons, hash code generation,
/// and cloning to be performed effectively for use in Entity Framework Core.
/// </remarks>
public class MultilingualTextComparer() : ValueComparer<MultilingualText>(
    (
        c1,
        c2
    ) => CompareDictionaries(c1, c2),
    c => ComputeHash(c),
    c => CloneMultilingualText(c)
)
{
    /// <summary>
    /// Compares two <see cref="MultilingualText"/> dictionaries for equality.
    /// </summary>
    /// <param name="d1">The first dictionary to compare. Can be null.</param>
    /// <param name="d2">The second dictionary to compare. Can be null.</param>
    /// <returns>
    /// True if the dictionaries are equal or both null; otherwise, false.
    /// </returns>
    private static bool CompareDictionaries(
        MultilingualText? d1,
        MultilingualText? d2
    )
    {
        if (ReferenceEquals(d1, d2))
        {
            return true;
        }

        if (d1 is null || d2 is null || d1.Count != d2.Count)
        {
            return false;
        }

        return d1.All(kv => d2.TryGetValue(kv.Key, out var value) && kv.Value == value);
    }

    /// <summary>
    /// Computes a hash code for a given <see cref="MultilingualText"/> object
    /// to enable its use in hash-based collections and comparisons.
    /// </summary>
    /// <param name="dictionary">
    /// A <see cref="MultilingualText"/> object representing a dictionary of language codes
    /// mapped to their respective text. If the value is null, the method will return a hash code of 0.
    /// </param>
    /// <returns>
    /// An integer representing the computed hash code for the given <see cref="MultilingualText"/>.
    /// </returns>
    private static int ComputeHash(
        MultilingualText? dictionary
    )
    {
        if (dictionary is null)
        {
            return 0;
        }

        unchecked
        {
            var hash = 17;
            foreach (var kv in dictionary.OrderBy(k => k.Key))
            {
                hash = (hash * 23) + kv.Key.GetHashCode();
                hash = (hash * 23) + kv.Value.GetHashCode();
            }

            return hash;
        }
    }

    /// <summary>
    /// Creates a deep copy of a given <see cref="MultilingualText"/> instance.
    /// </summary>
    /// <param name="source">The <see cref="MultilingualText"/> instance to clone.</param>
    /// <returns>A new <see cref="MultilingualText"/> instance containing the same key-value pairs as the source.</returns>
    private static MultilingualText CloneMultilingualText(
        MultilingualText source
    )
    {
        return new MultilingualText(
            source.ToDictionary(
                kv => kv.Key,
                kv => kv.Value
            )
        );
    }
}
