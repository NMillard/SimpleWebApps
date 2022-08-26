using Shared.Messaging;

namespace Consumer.WebApi;

public class DefaultMessage
{
    public Guid CorrelationId { get; set; }
}

public class DefaultMessageHandler : MessageHandler<DefaultMessage>
{
    private readonly ILogger<DefaultMessageHandler> logger;

    public DefaultMessageHandler(ILogger<DefaultMessageHandler> logger)
    {
        this.logger = logger;
        Topic = "message.created";
        ExchangeName = "messages";
        QueueName = "default-message-logger";
    }
    
    public override Task OnMessageReceived(DefaultMessage message)
    {
        logger.LogInformation("Received message with correlation id {CorrelationId}", message.CorrelationId.ToString());
        
        return Task.CompletedTask;
    }
}