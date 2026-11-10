# Acumen

Small library to write Rx.NET tests as marble diagrams.

## Install

Acumen is available as a set of Nuget packages.

### Which package do I need?

| Package | Use it when | Contains |
|---|---|---|
| `AcumenRx` | Always (it is pulled in by the packages below) | The marble DSL (`Cold`, `Hot`, `ExpectObservable`), `MarbleDiagram` and `TestSchedulerScope`. No test framework dependency. |
| `AcumenRx.Xunit` | Your tests use xUnit v2 (`xunit` 2.x) | The `[MarbleTest]` attribute for xUnit v2. |
| `AcumenRx.Xunit.v3` | Your tests use xUnit v3 (`xunit.v3`) | The `[MarbleTest]` attribute for xUnit v3. |

For xUnit v2:

``> dotnet add package AcumenRx.Xunit``

For xUnit v3:

``> dotnet add package AcumenRx.Xunit.v3``

Both attributes are called `MarbleTestAttribute` and live in the `Acumen` namespace, so tests look the same with either package:

```csharp
using System.Reactive.Linq;
using Acumen;
using static Acumen.Operators;

public class Example {
    [Fact, MarbleTest]
    public void Test() {
        var ints = Cold<int>(("---a--b--c-|", new { a = 1, b = 2, c = 3 }));

        ExpectObservable(ints.Select(i => i * 2),
            toBe: ("---a--b--c-|", new { a = 2, b = 4, c = 6 }));
    }
}
```

Without a test framework adapter you can create the scope yourself: `using var scope = new TestSchedulerScope();`.
The expectations are verified when the scope is disposed.

## Migrating from 0.15

Up to 0.15, the `AcumenRx` package depended on xUnit v2 and contained the `[MarbleTest]` attribute. As of 1.0.0, the
attribute lives in a separate package:

- **xUnit v2:** add `AcumenRx.Xunit` (you may drop the direct `AcumenRx` reference, it comes along). No code changes are needed.
- **xUnit v3:** replace `AcumenRx` with `AcumenRx.Xunit.v3`. No code changes are needed: `using Acumen;` and `[Fact, MarbleTest]` stay the same.
