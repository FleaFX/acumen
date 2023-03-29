using System.Collections.Immutable;

namespace Acumen.Extensions;

static class ExtensionsForImmutableStack {
    /// <summary>
    /// Attempts to peek the given <see cref="ImmutableStack"/> and returns a value indicating success or failure.
    /// </summary>
    /// <typeparam name="T">The type of contained element in the given stack.</typeparam>
    /// <param name="stack">The stack to attempt to peek.</param>
    /// <param name="value">The value on top of the stack, if the peek was successful.</param>
    /// <returns><c>true</c> if the stack was successfully peeked, otherwise <c>false</c>.</returns>
    public static bool TryPeek<T>(this ImmutableStack<T> stack, out T value) {
        var isEmpty = stack?.IsEmpty ?? true;
        value = isEmpty ? default(T) : stack.Peek();
        return !isEmpty;
    }
}