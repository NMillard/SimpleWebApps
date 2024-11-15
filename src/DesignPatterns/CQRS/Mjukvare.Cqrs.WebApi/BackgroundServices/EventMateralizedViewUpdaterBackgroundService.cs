using Microsoft.EntityFrameworkCore;
using Mjukvare.Cqrs.WebApi.DataLayer;
using Mjukvare.Cqrs.WebApi.Domain.ReadModels;

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
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
}

public sealed class ChannelMaterializedViewUpdaterBackgroundService(
    ILogger<ChannelMaterializedViewUpdaterBackgroundService> logger,
    MaterialViewUserCheckinUpdaterChannel channel,
    IServiceProvider serviceProvider
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (Guid id in channel.ReadAllNotificationUpdates(stoppingToken))
        {
            using IServiceScope scope = serviceProvider.CreateScope();
            await using var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            UserCheckinDisplay? checkin = await context.UserCheckinDisplays
                .AsNoTracking()
                .SingleOrDefaultAsync(u => u.UserId == id, stoppingToken);
            
            UserCheckinDisplay? result = await context.Users
                .AsNoTracking()
                .Include(u => u.Checkins)
                .Select(u => new UserCheckinDisplay
                {
                    UserId = u.Id,
                    Username = u.Username,
                    TotalCheckins = u.Checkins.Count,
                    LatestCheckinDate = u.Checkins.OrderByDescending(c => c.Created).Select(c => c.Created).FirstOrDefault(),
                    Checkins = u.Checkins.Select(c => new CheckinDisplay
                    {
                        CheckinId = c.Id,
                        Text = c.Text,
                        Created = c.Created
                    }).ToList()
                })
                .SingleOrDefaultAsync(u => u.UserId == id, stoppingToken);
            
            if (checkin is null)
            {
                context.UserCheckinDisplays.Add(result);
            }
            else
            {
                context.UserCheckinDisplays.Update(result);
            }

            try
            {
                await context.SaveChangesAsync(stoppingToken);
            }
            catch (DbUpdateException e)
            {
                logger.LogError(e, "Error updating materialized view for checkin {CheckinId}", id);
                continue;
            }
            
            logger.LogInformation("Updating materialized view for checkin {CheckinId}", id);
        }
    }
}