﻿using System.Reactive;
using System.Reflection;
using Acumen.Resources;
using Microsoft.Reactive.Testing;

namespace Acumen; 

public class MarbleDiagram<T> {
    readonly string _diagram;
    readonly object _legend;

    /// <summary>
    /// Initializes a new <see cref="MarbleDiagram{T}"/>.
    /// </summary>
    /// <param name="diagram">The string representation of the produced events.</param>
    /// <param name="legend">An object providing the values for the events in the diagram.</param>
    public MarbleDiagram(string diagram, object legend) {
        if (legend.GetType().GetRuntimeProperties().Any(p => p.PropertyType != typeof(T) && p.PropertyType.IsAssignableTo(typeof(Exception))))
            throw new ArgumentException(string.Format(MarbleDiagramResources.InvalidLegendObject, typeof(T)), nameof(legend));
        _diagram = diagram;
        _legend = legend;
    }

    /// <summary>
    /// Casts the given tuple to a <see cref="MarbleDiagram{T}"/>.
    /// </summary>
    /// <param name="tuple">The tuple to cast.</param>
    public static implicit operator MarbleDiagram<T>((string diagram, object legend) tuple) =>  new(tuple.diagram, tuple.legend);

    /// <summary>
    /// Casts the given <see cref="MarbleDiagram{T}"/> to an array of recorded notifications.
    /// </summary>
    /// <param name="diagram">The <see cref="MarbleDiagram{T}"/> to cast.</param>
    public static implicit operator Recorded<Notification<T>>[](MarbleDiagram<T> diagram) =>
        diagram.CreateRecordedNotifications().ToArray();

    IEnumerable<Recorded<Notification<T>>> CreateRecordedNotifications() {
        var unitOfTime = TestSchedulerScope.Current.UnitOfTime.Ticks;
        var steps = 0;
        var legend = _legend.GetType().GetRuntimeProperties().ToArray();
        foreach (var frame in _diagram) {
            switch (frame) {
                case '-':
                    steps++;
                    continue;

                case '#': {
                    var property = legend.Single(p => p.PropertyType.IsAssignableTo(typeof(Exception)));
                    yield return new Recorded<Notification<T>>(steps * unitOfTime, 
                        Notification.CreateOnError<T>((Exception)property.GetValue(_legend)!));
                    break;
                }

                case '|':
                    yield return new Recorded<Notification<T>>(steps * unitOfTime, Notification.CreateOnCompleted<T>());
                    break;

                default: {
                    var property = legend.Single(p => p.Name.Equals(frame.ToString(), StringComparison.OrdinalIgnoreCase));
                    yield return new Recorded<Notification<T>>(steps * unitOfTime,
                        Notification.CreateOnNext((T)property.GetValue(_legend)!));
                    break;
                }
            }
        }
    }
}