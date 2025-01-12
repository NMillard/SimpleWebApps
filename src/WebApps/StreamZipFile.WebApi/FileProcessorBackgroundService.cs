using System.Threading.Channels;

namespace StreamZipFile.WebApi;

public class FileProcessorBackgroundService : BackgroundService
{
    private readonly ProcessorChannel channel;
    private readonly ILogger<FileProcessorBackgroundService> logger;

    public FileProcessorBackgroundService(ProcessorChannel channel, ILogger<FileProcessorBackgroundService> logger)
    {
        this.channel = channel;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach(string e in channel.ReadAllAsync(stoppingToken))
        {
            logger.LogInformation("Processed element");
        }
    }
}


public class ProcessorChannel
{
    private readonly Channel<string> channel;
    private readonly ILogger<ProcessorChannel> logger;

    public ProcessorChannel(ILogger<ProcessorChannel> logger)
    {
        this.logger = logger;

        var options = new BoundedChannelOptions(100)
        {
            SingleWriter = false,
            SingleReader = true
        };

        channel = Channel.CreateBounded<string>(options);
    }

    public async Task<bool> AddElementAsync(string element, CancellationToken cancellationToken = default)
    {
        while (await channel.Writer.WaitToWriteAsync(cancellationToken) && !cancellationToken.IsCancellationRequested)
        {
            if (!channel.Writer.TryWrite(element)) continue;
            
            logger.LogInformation("Written to channel");
            return true;
        }
        
        return false;
    }

    public IAsyncEnumerable<string> ReadAllAsync(CancellationToken cancellationToken) => channel.Reader.ReadAllAsync(cancellationToken);
}