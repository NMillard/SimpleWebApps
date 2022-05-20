using SchedulerAgentSupervisor.WebApi.Domain;
using SchedulerAgentSupervisor.WebApi.Persistence;

namespace SchedulerAgentSupervisor.WebApi.BackgroundServices; 

public class ScheduleCheckerBackgroundService : BackgroundService {
    private readonly IServiceProvider provider;

    public ScheduleCheckerBackgroundService(IServiceProvider provider) => this.provider = provider;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        using IServiceScope scope = provider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        IEnumerable<IScheduledTaskDefinition> tasks = scope.ServiceProvider.GetServices<IScheduledTaskDefinition>();

        while (true) {
            IEnumerable<Schedule> dueSchedules = context.Schedules
                .AsEnumerable()
                .GetDueSchedules(DateTimeOffset.UtcNow);

            foreach (Schedule schedule in dueSchedules) {
                RunContext runContext = schedule.StartRun();
                
                IScheduledTaskDefinition? task = tasks.GetByName(schedule.TaskName);
                if (task is null) continue;

                try {
                    await task.RunAsync();
                    runContext.Finished();
                } catch (Exception e) {
                    Console.WriteLine(e);
                    throw;
                }
                
                context.Schedules.Update(schedule);
            }

            context.SaveChangesAsync();
            await Task.Delay(5000);
        }
    }
}


internal static class ScheduleExtensions {
    public static IScheduledTaskDefinition? GetByName(this IEnumerable<IScheduledTaskDefinition> tasks, string name)
        => tasks.SingleOrDefault(t => t.GetType().Name.Equals(name));

    public static IEnumerable<Schedule> GetDueSchedules(this IEnumerable<Schedule> schedules, DateTimeOffset time)
        => schedules.Where(s => s.IsDue(time));
}