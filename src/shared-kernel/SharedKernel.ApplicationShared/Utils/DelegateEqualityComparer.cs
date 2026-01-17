namespace SharedKernel.ApplicationShared.Utils;

/// <summary>
/// A generic equality comparer that utilizes delegate functions for determining equality
/// and generating hash codes for objects of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of objects to be compared.</typeparam>
/// <remarks>
/// This class allows for custom equality and hash code logic without the need to implement
/// IEqualityComparer in a dedicated class. Use provided delegates to define these behaviors.
/// </remarks>
/// <threadsafety>
/// Instances of this class are not inherently thread-safe. Ensure synchronization when using shared instances concurrently across threads.
/// </threadsafety>
/// <seealso cref="System.Collections.Generic.IEqualityComparer{T}"/>
public class DelegateEqualityComparer<T>(
    Func<T?, T?, bool> equals,
    Func<T, int> getHashCode
) : IEqualityComparer<T>
{
    /// <summary>
    /// A private readonly delegate function used to encapsulate the custom logic for determining equality between two instances of type <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>
    /// This delegate is initialized during the construction of the <see cref="DelegateEqualityComparer{T}"/> instance
    /// and is invoked when implementing the <see cref="IEqualityComparer{T}.Equals(T, T)"/> method.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// Thrown during initialization if a null delegate is provided for the equality comparison mechanism.
    /// </exception>
    private readonly Func<T?, T?, bool> _equals = equals ?? throw new ArgumentNullException(nameof(equals));

    /// <summary>
    /// A private field that stores the delegate responsible for computing the hash code of an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>
    /// This field is initialized through the constructor of the <see cref="DelegateEqualityComparer{T}"/> class, where a custom delegate
    /// is supplied to define the logic for hash code computation. It is used internally by the <see cref="GetHashCode"/> method.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// Thrown during initialization if a null delegate is provided for the hash code computation.
    /// </exception>
    private readonly Func<T, int> _getHashCode = getHashCode ?? throw new ArgumentNullException(nameof(getHashCode));

    /// Determines whether the specified objects are equal.
    /// <param name="x">The first object to compare.</param>
    /// <param name="y">The second object to compare.</param>
    /// <returns>True if the specified objects are equal; otherwise, false.</returns>
    public bool Equals(
        T? x,
        T? y
    )
    {
        return _equals(x, y);
    }

    /// Generates a hash code for the specified object using the provided hash function.
    /// <param name="obj">The object for which the hash code is to be generated.</param>
    /// <returns>An integer representing the hash code of the specified object.</returns>
    public int GetHashCode(
        T obj
    )
    {
        return _getHashCode(obj);
    }
}
