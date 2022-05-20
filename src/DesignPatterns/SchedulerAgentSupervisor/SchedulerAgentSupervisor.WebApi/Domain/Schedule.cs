using Azure.Storage.Queues;

namespace SchedulerAgentSupervisor.WebApi.Domain; 

public class Schedule {
    private List<ScheduleRun> history;

    public Schedule() {
        history = new List<ScheduleRun>();
    }

    public Guid Id { get; set; }
    public string TaskName { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public TimeOnly TimeOfDay { get; set; }
    public int IntervalInDays { get; set; }

    public IEnumerable<ScheduleRun> History => history.AsReadOnly();

    public RunContext StartRun() => new RunContext(Guid.NewGuid(), this);

    internal void AddRunEntry(ScheduleRun run) => history.Add(run);

    public bool IsDue(DateTimeOffset time) {
        return true;
    }
}


public class ScheduleRun {
    public string TaskName { get; set; }
    public DateTimeOffset Time { get; set; }
    public Guid CorrelationId { get; set; }
    public RunState State { get; set; }
}

public enum RunState {
    Started,
    Finished,
}

public record RunContext {
    public RunContext(Guid correlationId, Schedule schedule) {
        CorrelationId = correlationId;
        Schedule = schedule;
        
        Schedule.AddRunEntry(new ScheduleRun {
            Time = DateTimeOffset.UtcNow,
            CorrelationId = CorrelationId,
            TaskName = Schedule.TaskName,
            State = RunState.Started
        });
    }
    
    public Guid CorrelationId { get; init; }
    public Schedule Schedule { get; init; }

    public void Finished() =>
        Schedule.AddRunEntry(new ScheduleRun {
            Time = DateTimeOffset.UtcNow,
            CorrelationId = CorrelationId,
            TaskName = Schedule.TaskName,
            State = RunState.Finished
        });
    
    public void Deconstruct(out Guid CorrelationId, out Schedule Schedule) {
        CorrelationId = this.CorrelationId;
        Schedule = this.Schedule;
    }
}

public interface IScheduledTaskDefinition {
    Task RunAsync();
}

public class SayHelloTask : IScheduledTaskDefinition {
    private readonly ILogger<SayHelloTask> logger;
    public SayHelloTask(ILogger<SayHelloTask> logger) => this.logger = logger;

    public Task RunAsync() {
        logger.LogInformation("Hello there!");
        
        return Task.CompletedTask;
    }
}

public class QueueTask : IScheduledTaskDefinition {
    private readonly QueueClient client;
    private readonly ILogger<QueueTask> logger;

    public QueueTask(QueueServiceClient client, ILogger<QueueTask> logger) {
        this.client = client.GetQueueClient("scheduler");
        this.logger = logger;
    }

    public async Task RunAsync() {
        logger.LogInformation("QueueTask called");
        await client.SendMessageAsync("hello there");
    }
}