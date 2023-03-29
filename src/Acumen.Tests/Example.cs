using System.Reactive.Linq;

namespace Acumen;

using static Operators;

public class Example {

    [Fact, MarbleTest]
    public void Test() {
        var ints = Cold<int>(("---a--b--c-|", new { a = 1, b = 2,  c = 3 }));
        
        Expect(ints.Select(i => i * 2),
            toBe: ("---a--b--c-|", new { a = 2, b = 4, c = 6 })
        );
    }
}