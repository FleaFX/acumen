using KellermanSoftware.CompareNetObjects;

namespace Acumen;

public class CompareNetObjectsComparer<T> : IEqualityComparer<T> {
    readonly CompareLogic _compare;

    /// <summary>
    /// Initializes a new <see cref="CompareNetObjectsComparer{T}"/>.
    /// </summary>
    public CompareNetObjectsComparer() => _compare = new CompareLogic();

    /// <summary>Determines whether the specified objects are equal.</summary>
    /// <param name="x">The first object of type <paramref name="T" /> to compare.</param>
    /// <param name="y">The second object of type <paramref name="T" /> to compare.</param>
    /// <returns>
    /// <see langword="true" /> if the specified objects are equal; otherwise, <see langword="false" />.</returns>
    public bool Equals(T? x, T? y) =>
        _compare.Compare(x, y).AreEqual;

    /// <summary>Returns a hash code for the specified object.</summary>
    /// <param name="obj">The <see cref="T:System.Object" /> for which a hash code is to be returned.</param>
    /// <exception cref="T:System.ArgumentNullException">The type of <paramref name="obj" /> is a reference type and <paramref name="obj" /> is <see langword="null" />.</exception>
    /// <returns>A hash code for the specified object.</returns>
    public int GetHashCode(T obj) =>
        obj.GetHashCode();
}