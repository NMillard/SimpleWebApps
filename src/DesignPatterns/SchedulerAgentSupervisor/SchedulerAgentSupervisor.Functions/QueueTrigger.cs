using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace SchedulerAgentSupervisor.Functions; 

public class QueueTrigger {
    
    [Function("QueueTrigger")]
    public void Run(
        [QueueTrigger("scheduler", Connection = "AzureWebJobsStorage")] string myQueueItem,
        FunctionContext context) {
        ILogger logger = context.GetLogger("QueueTrigger");
        
        logger.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        
    }
}