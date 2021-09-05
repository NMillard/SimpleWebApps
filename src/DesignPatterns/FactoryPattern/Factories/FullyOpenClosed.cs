using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CompositionOverInheritance.Composition;

namespace FactoryPattern.Factories.FullyOpenClosed {
    public class ScheduleFactoryProvider {
        private readonly Dictionary<string, Type> factories;

        public ScheduleFactoryProvider() {
            factories = Assembly.GetAssembly(typeof(ScheduleFactoryProvider))?
                .GetTypes()
                .Where(t => t.GetInterfaces().Any(i => i.GetGenericTypeDefinition() == typeof(ITriggerTimeFactory<>)))
                .ToDictionary(
                    keySelector: t =>
                        t.GetCustomAttribute<FactoryName>()?.Name.ToLower() ?? throw new ArgumentException()
                );
        }

        public string[] RegisteredFactories => factories.Keys.ToArray();

        public Schedule Create<T>(string factoryName, T parameters) where T : ScheduleParameters {
            Type factoryType = factories[factoryName.ToLower()];
            if (factoryType is null) throw new InvalidOperationException();

            var factory = Activator.CreateInstance(factoryType) as ITriggerTimeFactory<T>;
            ITriggerTime triggerTime = factory?.Create(parameters) ?? throw new ArgumentException();

            return new Schedule(triggerTime);
        }
    }


    public interface ITriggerTimeFactory<in TParams> where TParams : ScheduleParameters {
        ITriggerTime Create(TParams parameters);
    }

    [FactoryName("Daily")]
    public class DailyTriggerTimeFactory : ITriggerTimeFactory<EmptyParameters> {
        public ITriggerTime Create(EmptyParameters parameters) => new DailyTrigger();
    }

    [FactoryName("EndOfWeek")]
    public class EndOfWeekTriggerTimeFactory : ITriggerTimeFactory<EmptyParameters> {
        public ITriggerTime Create(EmptyParameters parameters) => new EndOfWeekTrigger();
    }

    [FactoryName("Weekly")]
    public class WeeklyTriggerTimeFactory : ITriggerTimeFactory<WeeklyParameters> {
        public ITriggerTime Create(WeeklyParameters parameters) => new WeeklyTrigger(parameters.DayOfWeek);
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class FactoryName : Attribute {
        public FactoryName(string name) {
            Name = name;
        }

        public string Name { get; }
    }
}