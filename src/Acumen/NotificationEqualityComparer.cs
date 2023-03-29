using System.Reactive;

namespace Acumen;

class NotificationEqualityComparer<T> : IEqualityComparer<Notification<T>> {
    /// <summary>Determines whether the specified objects are equal.</summary>
    /// <param name="x">The first object of type <paramref name="T" /> to compare.</param>
    /// <param name="y">The second object of type <paramref name="T" /> to compare.</param>
    /// <returns>
    /// <see langword="true" /> if the specified objects are equal; otherwise, <see langword="false" />.</returns>
    public bool Equals(Notification<T>? x, Notification<T>? y) =>
        x.Kind switch {
            NotificationKind.OnCompleted => y.Kind == NotificationKind.OnCompleted,
            NotificationKind.OnError => y.Kind == NotificationKind.OnError && Equals(x.Exception, y.Exception),
            _ => y.Kind == NotificationKind.OnNext && typeof(T).IsAssignableTo(typeof(IEnumerable<T>))
                ? ((IEnumerable<T>)x.Value).SequenceEqual((IEnumerable<T>)y.Value)
                : Equals(x.Value, y.Value)
        };

    /// <summary>Returns a hash code for the specified object.</summary>
    /// <param name="obj">The <see cref="T:System.Object" /> for which a hash code is to be returned.</param>
    /// <exception cref="T:System.ArgumentNullException">The type of <paramref name="obj" /> is a reference type and <paramref name="obj" /> is <see langword="null" />.</exception>
    /// <returns>A hash code for the specified object.</returns>
    public int GetHashCode(Notification<T> obj) => obj.GetHashCode();
}