using System.Reactive.Linq;
using System.Reflection;
#if XUNIT_V3
using Xunit.v3;
#endif

namespace Acumen;

using static Operators;

// Compiled into both the xUnit v2 and the xUnit v3 test project, so both adapters are held to the same behavior.
public class MarbleTestAttributeTests {

    [Fact, MarbleTest]
    public void Current_scope_is_available_in_a_marble_test() =>
        Assert.NotNull(TestSchedulerScope.Current);

    [Fact, MarbleTest]
    public async Task Current_scope_flows_into_an_async_marble_test() {
        var scope = TestSchedulerScope.Current;

        await Task.Yield();
        Assert.Same(scope, TestSchedulerScope.Current);

        await Task.Delay(1, CancellationToken.None);
        Assert.Same(scope, TestSchedulerScope.Current);

        // The expectation is registered after the awaits and must still be verified by the attribute's After.
        ExpectObservable(Cold<int>(("-a-|", new { a = 1 })), toBe: ("-a-|", new { a = 1 }));
    }

    [Fact, MarbleTest]
    public void Matching_cold_expectation_passes() =>
        ExpectObservable(Cold<int>(("-a--b-|", new { a = 1, b = 2 })).Select(i => i * 10),
            toBe: ("-a--b-|", new { a = 10, b = 20 }));

    [Fact, MarbleTest]
    public void Matching_hot_expectation_passes() =>
        ExpectObservable(Hot<int>(("--a-b--|", new { a = 1, b = 2 })).Where(i => i > 1),
            toBe: ("---b--|", new { b = 2 }));

    [Fact]
    public void Mismatching_expectation_makes_After_throw() {
        var attribute = new MarbleTestAttribute();
        Before(attribute);

        ExpectObservable(Cold<int>(("-a-|", new { a = 1 })), toBe: ("-a-|", new { a = 2 }));

        Assert.Throws<AssertionException>(() => After(attribute));
    }

    [Fact]
    public void Matching_expectation_does_not_make_After_throw() {
        var attribute = new MarbleTestAttribute();
        Before(attribute);

        ExpectObservable(Cold<int>(("-a-|", new { a = 1 })), toBe: ("-a-|", new { a = 1 }));

        After(attribute);
    }

    [Fact]
    public void Operators_without_MarbleTest_fail_with_a_helpful_message() {
        var exception = Assert.Throws<InvalidOperationException>(() => Cold<int>("-|"));
        Assert.Contains("not decorated with the [MarbleTest] attribute", exception.Message);
    }

    static readonly MethodInfo ThisMethod = typeof(MarbleTestAttributeTests).GetMethod(nameof(Operators_without_MarbleTest_fail_with_a_helpful_message))!;

#if XUNIT_V3
    static void Before(MarbleTestAttribute attribute) => attribute.Before(ThisMethod, (IXunitTest)TestContext.Current.Test!);
    static void After(MarbleTestAttribute attribute) => attribute.After(ThisMethod, (IXunitTest)TestContext.Current.Test!);
#else
    static void Before(MarbleTestAttribute attribute) => attribute.Before(ThisMethod);
    static void After(MarbleTestAttribute attribute) => attribute.After(ThisMethod);
#endif
}
