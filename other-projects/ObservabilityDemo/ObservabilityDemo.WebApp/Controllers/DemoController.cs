using System.Diagnostics;
using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Mvc;

namespace ObservabilityDemo.WebApp.Controllers;

[Route("api/demo")]
public class DemoController(
    ILogger<DemoController> logger 
    // ,IMeterFactory meterFactory
    ) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<dynamic>> GetDemo()
    {
        // Meter meter = meterFactory.Create("MyService");

        var source = new ActivitySource("MyService");
        Activity? activity = source.StartActivity();
        activity?.AddEvent(new ActivityEvent("hello!"));
        
        logger.LogCall();
        
        var objects = new List<dynamic>
        {
            new
            {
                Hello = "There"
            },
            new
            {
                Hello = "Yes"
            }
        };

        // meter.CreateCounter<int>("myapp.democontroller.call", "endpoint called");
        return Ok(objects);
    }
}

public static partial class LogExtensions
{
    [LoggerMessage(LogLevel.Information, message: "Get demo called!")]
    public static partial void LogCall(this ILogger logger);
}
