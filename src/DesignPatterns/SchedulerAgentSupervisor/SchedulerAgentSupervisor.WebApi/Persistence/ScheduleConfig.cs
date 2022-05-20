using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchedulerAgentSupervisor.WebApi.Domain;

namespace SchedulerAgentSupervisor.WebApi.Persistence;

public class ScheduleConfig : IEntityTypeConfiguration<Schedule> {
    public void Configure(EntityTypeBuilder<Schedule> builder) {
        builder.ToTable("Schedules");

        builder.HasKey(p => p.Id);
        builder.Property(s => s.TaskName).HasMaxLength(150).IsRequired();
        builder.Property(s => s.IntervalInDays).IsRequired();
        builder.Property(s => s.TimeOfDay).HasConversion<TimeOnlyConverter>().HasColumnType("TIME").IsRequired();
        builder.Property(s => s.StartDate).HasConversion<DateOnlyConverter>().HasColumnType("DATE").IsRequired();
        builder.Property(s => s.EndDate).HasConversion<DateOnlyConverter>().HasColumnType("DATE");

        builder.HasIndex(s => s.TaskName).IsUnique();
        
        builder.OwnsMany<ScheduleRun>(s => s.History, runBuilder => {
            runBuilder.ToTable("ScheduleRuns");

            runBuilder.HasKey(r => new { r.TaskName, r.CorrelationId, r.State });

            runBuilder.Property(r => r.Time).IsRequired();
            runBuilder.Property(r => r.CorrelationId).IsRequired();
            runBuilder.Property(r => r.TaskName).HasMaxLength(150).IsRequired();
            runBuilder.Property(r => r.State).HasConversion<string>().IsRequired();
        });
    }
}