namespace Mjukvare.Cqrs.WebApi.BackgroundServices;

public sealed class EventMateralizedViewUpdaterBackgroundService(
    ReadUpdateNotifier notifier,
    ILogger<EventMateralizedViewUpdaterBackgroundService> logger
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        notifier.CheckinAdded += (sender, args) => { logger.LogInformation("Updating materialized view"); };

        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
}

public sealed class ChannelMaterializedViewUpdaterBackgroundService(
    ILogger<ChannelMaterializedViewUpdaterBackgroundService> logger,
    MaterialViewUserCheckinUpdaterChannel channel,
    IServiceProvider serviceProvider
)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (Guid id in channel.ReadAllNotificationUpdates(stoppingToken))
        {
            logger.LogInformation("Updating materialized view for checkin {CheckinId}", id);
        }
    }
}