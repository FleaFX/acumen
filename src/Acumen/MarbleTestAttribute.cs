using System.Reflection;
using Xunit.Sdk;

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
    public override void Before(MethodInfo methodUnderTest) =>
        _scope = new TestSchedulerScope();
    
    /// <summary>
    /// This method is called after the test method is executed.
    /// </summary>
    /// <param name="methodUnderTest">The method under test</param>
    public override void After(MethodInfo methodUnderTest) =>
        _scope?.Dispose();
}