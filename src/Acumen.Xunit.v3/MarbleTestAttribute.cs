using System.Reflection;
using Xunit.v3;

namespace Acumen;

/// <summary>
/// Attribute that sets up a test method with marble testing infrastructure.
/// </summary>
public sealed class MarbleTestAttribute : BeforeAfterTestAttribute {
    TestSchedulerScope? _scope;

    /// <summary>
    /// This method is called before the test method is executed.
    /// </summary>
    /// <param name="methodUnderTest">The method under test</param>
    /// <param name="test">The current test</param>
    public override void Before(MethodInfo methodUnderTest, IXunitTest test) =>
        _scope = new TestSchedulerScope();

    /// <summary>
    /// This method is called after the test method is executed.
    /// </summary>
    /// <param name="methodUnderTest">The method under test</param>
    /// <param name="test">The current test</param>
    public override void After(MethodInfo methodUnderTest, IXunitTest test) =>
        _scope?.Dispose();
}
