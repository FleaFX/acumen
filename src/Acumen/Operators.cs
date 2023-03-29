using System.Reactive;
using Acumen.Resources;
using Microsoft.Reactive.Testing;

namespace Acumen; 

public static class Operators {
    /// <summary>
    /// Creates a cold observable using the given <paramref name="marbleDiagram"/>.
    /// </summary>
    /// <typeparam name="T">The type of the elements produced by the resulting observable.</typeparam>
    /// <param name="marbleDiagram">The string representation of the events produced by the observable.</param>
    /// <returns>A <see cref="IObservable{T}"/>.</returns>
    public static IObservable<T> Cold<T>(MarbleDiagram<T> marbleDiagram) =>
        TestSchedulerScope.Current.Scheduler.CreateColdObservable<T>(marbleDiagram);

    /// <summary>
    /// Creates a hot observable using the given <paramref name="marbleDiagram"/>.
    /// </summary>
    /// <typeparam name="T">The type of the elements produced by the resulting observable.</typeparam>
    /// <param name="marbleDiagram">The string representation of the events produced by the observable.</param>
    /// <returns>A <see cref="IObservable{T}"/>.</returns>
    public static IObservable<T> Hot<T>(MarbleDiagram<T> marbleDiagram) =>
        TestSchedulerScope.Current.Scheduler.CreateHotObservable<T>(marbleDiagram);

    /// <summary>
    /// Schedules an assertion to be performed when the test scheduler starts.
    /// </summary>
    /// <typeparam name="T">The type of the elements produced by the asserted observable.</typeparam>
    /// <param name="observable">The observable to assert.</param>
    /// <param name="toBe">The <see cref="MarbleDiagram{T}"/> of the events that are expected to be produced by the asserted observable.</param>
    /// <exception cref="AssertionException">Thrown when the asserted observable does not match the expected marble diagram.</exception>
    public static void ExpectObservable<T>(IObservable<T> observable, MarbleDiagram<T> toBe) {
        TestSchedulerScope.Current.ScheduleAssertion(scheduler => {
            Recorded<Notification<T>>[]  expectedNotifications = toBe;
            
            var end = expectedNotifications.Last().Time + 1000;
            
            // start the scheduler and observe notifications
            var observer = scheduler.Start(() => observable, 0, 0, end);

            // assert that the observer contains the expected notifications
            if (observer.Messages.Count != expectedNotifications.Length)
                throw new AssertionException(string.Format(OperatorResources.NotificationCountMismatch, expectedNotifications.Length, observer.Messages.Count));

            var comparer = new RecordedEqualityComparer<Notification<T>>(new NotificationEqualityComparer<T>());
            for (var i = 0; i < observer.Messages.Count; i++) {
                if (!comparer.Equals(observer.Messages[i], expectedNotifications[i]))
                    throw new AssertionException(string.Format(OperatorResources.NotificationInequality, i + 1, expectedNotifications[i], observer.Messages[i]));
            }
        });
    }

    
}