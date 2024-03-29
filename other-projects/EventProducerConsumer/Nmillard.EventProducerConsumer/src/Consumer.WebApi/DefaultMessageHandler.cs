﻿using Shared.Messaging;

namespace Consumer.WebApi;

internal class DefaultMessageHandler : MessageHandler<DefaultMessage>
{
    private readonly ILogger<DefaultMessageHandler> logger;

    public DefaultMessageHandler(ILogger<DefaultMessageHandler> logger)
    {
        this.logger = logger;
        Topic = "message.created";
        ExchangeName = "messages";
        QueueName = "default-message-logger";
    }
    
    public override async Task OnMessageReceivedAsync(DefaultMessage message)
    {
        logger.LogInformation("Received message with correlation id {CorrelationId}", message.CorrelationId.ToString());
    }
}

public class DefaultMessage
{
    public Guid CorrelationId { get; set; }
}