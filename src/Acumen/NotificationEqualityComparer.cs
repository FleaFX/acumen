using System.Reactive;
using System.Reflection;

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
            _ => y.Kind == NotificationKind.OnNext && (TryCompareEnumerable(x.Value, y.Value) || CompareDiscrete(x.Value, y.Value))
        };

    static bool TryCompareEnumerable(T x, T y) {
        var type = typeof(T);

        if (type.GenericTypeArguments.Length == 1 && type.IsAssignableTo(typeof(IEnumerable<>).MakeGenericType(type.GenericTypeArguments))) {
            var sequenceEqual = typeof(Enumerable).GetRuntimeMethods().First(m => m.Name == "SequenceEqual").MakeGenericMethod(type.GenericTypeArguments);
            return (bool)sequenceEqual.Invoke(null, new object[] { x, y }); 
        }

        if (type.IsArray) {
            var sequenceEqual = typeof(Enumerable).GetRuntimeMethods().First(m => m.Name == "SequenceEqual").MakeGenericMethod(type.GetElementType()!);
            return (bool)sequenceEqual.Invoke(null, new object[] { x, y });
        }

        return false;
    }
    static bool CompareDiscrete(T x, T y) => Equals(x, y);
    
    static bool IsEnumerable() {
        var type = typeof(T);
        return type.GenericTypeArguments.Length == 1
            ? type.IsAssignableTo(typeof(IEnumerable<>).MakeGenericType(type.GenericTypeArguments[0]))
            : type.IsArray;
    }

    /// <summary>Returns a hash code for the specified object.</summary>
    /// <param name="obj">The <see cref="T:System.Object" /> for which a hash code is to be returned.</param>
    /// <exception cref="T:System.ArgumentNullException">The type of <paramref name="obj" /> is a reference type and <paramref name="obj" /> is <see langword="null" />.</exception>
    /// <returns>A hash code for the specified object.</returns>
    public int GetHashCode(Notification<T> obj) => obj.GetHashCode();
}