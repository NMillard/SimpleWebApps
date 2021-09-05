using System;
using System.Collections.Generic;
using System.Linq;
using CompositionOverInheritance.Composition;

namespace FactoryPattern.Factories.DictionaryBased {
    public class ScheduleFactory {
        private readonly Dictionary<string, Func<ScheduleParameters, ITriggerTime>> factories;

        public ScheduleFactory() {
            factories = new Dictionary<string, Func<ScheduleParameters, ITriggerTime>>();
        }

        public string[] RegisteredFactories => factories.Keys.ToArray();
        
        public Schedule Create(string type, ScheduleParameters parameters) => new Schedule(factories[type.ToLower()](parameters));

        public void RegisterSchedule(string type, Func<ScheduleParameters, ITriggerTime> factory) {
            if (string.IsNullOrEmpty(type)) return;
            factories[type.ToLower()] = factory;
        }
    }
}