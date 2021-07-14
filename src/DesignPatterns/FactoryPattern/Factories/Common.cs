using System;

namespace FactoryPattern.Factories {
    public abstract record ScheduleParameters {
        public static EmptyParameters Empty => new();
    }

    public sealed record EmptyParameters : ScheduleParameters {
        internal EmptyParameters() {}
    }

    public sealed record WeeklyParameters(DayOfWeek DayOfWeek) : ScheduleParameters;
}