using System.Collections.Immutable;
using Acumen.Extensions;
using Acumen.Resources;
using Microsoft.Reactive.Testing;

namespace Acumen;

sealed class TestSchedulerScope : IDisposable {
    readonly List<Action<TestScheduler>> _assertions = new();

    /// <summary>
    /// Gets the <see cref="TestScheduler"/> associated with this scope.
    /// </summary>
    internal TestScheduler Scheduler { get; } = new();

    /// <summary>
    /// Schedules the given <see cref="Action"/> to run 
    /// </summary>
    /// <param name="assertion"></param>
    internal void ScheduleAssertion(Action<TestScheduler> assertion) =>
        _assertions.Add(assertion);

    /// <summary>
    /// Gets the amount of time that passes for each frame in the diagram.
    /// </summary>
    public TimeSpan UnitOfTime { get; }

    /// <summary>
    /// Initializes a new <see cref="TestSchedulerScope"/>.
    /// </summary>
    public TestSchedulerScope(TimeSpan? unitOfTime = null) {
        UnitOfTime = unitOfTime ?? TimeSpan.FromMilliseconds(10);
        Scopes = Scopes.Push(this);
    }

    static readonly string ThreadId = Guid.NewGuid().ToString("N");
    static ImmutableStack<TestSchedulerScope> Scopes {
        get => CallContext<ImmutableStack<TestSchedulerScope>>.LogicalGetData(ThreadId) ?? ImmutableStack.Create<TestSchedulerScope>();
        set => CallContext<ImmutableStack<TestSchedulerScope>>.LogicalSetData(ThreadId, value);
    }

    /// <summary>
    /// Gets the innermost (in case of nested scopes) <see cref="TestSchedulerScope"/>.
    /// </summary>
    public static TestSchedulerScope Current {
        get {
            if (!Scopes.TryPeek(out var scope))
                throw new InvalidOperationException(TestSchedulerScopeResources.NoCurrentTestSchedulerScopeAvailable);
            return scope;
        }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    void Dispose(bool disposing) {
        if (disposing) {
            if (Scopes.TryPeek(out var scope)) {
                scope._assertions.ForEach(assertion => assertion(Scheduler));
                Scopes = Scopes.Pop(out _);
            };
        }
    }
}