namespace Acumen; 

/// <summary>
/// Exception thrown when an asserted observable does not match an expected marble diagram.
/// </summary>
public class AssertionException : Exception {
    /// <summary>
    /// Initializes a new <see cref="AssertionException"/>.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public AssertionException(string message) : base(message) { }
}