using Microsoft.AspNetCore.Mvc;
using SchedulerAgentSupervisor.WebApi.Domain;
using SchedulerAgentSupervisor.WebApi.Persistence;

namespace SchedulerAgentSupervisor.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class SchedulesController : ControllerBase {
    private readonly AppDbContext context;
    private readonly ILogger<SchedulesController> _logger;

    public SchedulesController(AppDbContext context, ILogger<SchedulesController> logger) {
        this.context = context;
        _logger = logger;
    }


    [HttpGet(Name = "GetSchedules")]
    public IActionResult Get() {
        List<Schedule> schedules = context.Schedules.ToList();
        return Ok(schedules);
    }

    [HttpPost(Name = "CreateSchedule")]
    public async Task<IActionResult> Create() {
        context.Schedules.Add(new Schedule {
            Id = Guid.NewGuid(),
            EndDate = new DateOnly(2022, 7, 30),
            StartDate = new DateOnly(2022, 5, 4),
            TimeOfDay = TimeOnly.Parse("10:00:00"),
            TaskName = "SomeTask",
            IntervalInDays = 5
        });
        await context.SaveChangesAsync();

        return Ok();
    }
}