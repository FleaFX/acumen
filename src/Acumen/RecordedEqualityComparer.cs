using Microsoft.Reactive.Testing;

namespace Acumen;

/// <summary>
/// Compares two <see cref="Recorded{T}"/> records for equality.
/// </summary>
/// <typeparam name="T"></typeparam>
sealed class RecordedEqualityComparer<T> : IEqualityComparer<Recorded<T>> {
    readonly IEqualityComparer<T>? _valueComparer;

    /// <summary>
    /// Initializes a new <see cref="RecordedEqualityComparer{T}"/>.
    /// </summary>
    /// <param name="valueComparer">An optional <see cref="IEqualityComparer{T}"/> that compares the value of the <see cref="Recorded{T}"/>.</param>
    public RecordedEqualityComparer(IEqualityComparer<T>? valueComparer = null) =>
        _valueComparer = valueComparer;

    /// <summary>Determines whether the specified objects are equal.</summary>
    /// <param name="x">The first object of type <paramref name="T" /> to compare.</param>
    /// <param name="y">The second object of type <paramref name="T" /> to compare.</param>
    /// <returns>
    /// <see langword="true" /> if the specified objects are equal; otherwise, <see langword="false" />.</returns>
    public bool Equals(Recorded<T> x, Recorded<T> y) =>
        (x.Time - (x.Time % 10)) == (y.Time - (y.Time % 10)) && (_valueComparer?.Equals(x.Value, y.Value) ?? Equals(x.Value, y.Value));

    /// <summary>Returns a hash code for the specified object.</summary>
    /// <param name="obj">The <see cref="T:System.Object" /> for which a hash code is to be returned.</param>
    /// <exception cref="T:System.ArgumentNullException">The type of <paramref name="obj" /> is a reference type and <paramref name="obj" /> is <see langword="null" />.</exception>
    /// <returns>A hash code for the specified object.</returns>
    public int GetHashCode(Recorded<T> obj) =>
        obj.GetHashCode();
}