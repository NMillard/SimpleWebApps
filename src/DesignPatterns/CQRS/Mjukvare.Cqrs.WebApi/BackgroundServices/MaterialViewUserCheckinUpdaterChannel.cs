using System.Threading.Channels;

namespace Mjukvare.Cqrs.WebApi.BackgroundServices;

public class MaterialViewUserCheckinUpdaterChannel
{
    private const int MaxMessagesInChannel = 100;
    private readonly Channel<Guid> channel;
    private readonly ILogger<MaterialViewUserCheckinUpdaterChannel> logger;

    public MaterialViewUserCheckinUpdaterChannel(ILogger<MaterialViewUserCheckinUpdaterChannel> logger)
    {
        this.logger = logger;

        var options = new BoundedChannelOptions(MaxMessagesInChannel)
        {
            SingleWriter = false,
            SingleReader = true
        };

        channel = Channel.CreateBounded<Guid>(options);
    }

    public async Task<bool> NotifyMaterializedViewUpdate(Guid checkinId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Notifying materialized view should update");

        while (await channel.Writer.WaitToWriteAsync(cancellationToken) && !cancellationToken.IsCancellationRequested)
        {
            if (!channel.Writer.TryWrite(checkinId)) continue;
            
            logger.LogInformation("Notified materialized view update");
            return true;
        }

        return false;
    }

    public IAsyncEnumerable<Guid> ReadAllNotificationUpdates(CancellationToken cancellationToken = default)
    {
        return channel.Reader.ReadAllAsync(cancellationToken);
    }
}