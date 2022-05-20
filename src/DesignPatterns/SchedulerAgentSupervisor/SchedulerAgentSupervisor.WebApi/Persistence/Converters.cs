using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace SchedulerAgentSupervisor.WebApi.Persistence;

public class DateOnlyConverter : ValueConverter<DateOnly, DateTime> {
    public DateOnlyConverter() : base(
        (date) => date.ToDateTime(TimeOnly.MinValue),
        (date) => DateOnly.FromDateTime(date) 
    ) {}
}


public class TimeOnlyConverter : ValueConverter<TimeOnly, TimeSpan> {
    public TimeOnlyConverter() : base(
        timeOnly => timeOnly.ToTimeSpan(),
        timeSpan => TimeOnly.FromTimeSpan(timeSpan)
    ) { }
}