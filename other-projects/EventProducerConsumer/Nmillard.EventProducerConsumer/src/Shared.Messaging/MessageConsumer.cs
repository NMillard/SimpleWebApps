using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Shared.Messaging;

public abstract class MessageHandler<TMessage>
{
    public string ExchangeName { get; protected init; }
    public string QueueName { get; protected init; }
    public string Topic { get; protected init; }
    public abstract Task OnMessageReceivedAsync(TMessage message);
}

internal class MessageConsumer<THandler, TMessage> : BackgroundService
    where THandler : MessageHandler<TMessage>
{
    private readonly THandler handler;
    private readonly RabbitMqConnection connection;

    public MessageConsumer(THandler handler, RabbitMqConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);
        this.handler = handler;
        this.connection = connection;
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Notice I'm not applying the "using" keyword when creating the model.
        // Doing so would dispose the model when the method returns and results in a closed channel.
        IModel channel = connection.Connection.CreateModel();
        channel.ExchangeDeclare(handler.ExchangeName, ExchangeType.Topic, durable: true);
        channel.QueueDeclare(handler.QueueName, durable: true, exclusive: false);
        channel.QueueBind(handler.QueueName, handler.ExchangeName, handler.Topic);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (_, args) =>
        {
            byte[] body = args.Body.ToArray();
            string message = Encoding.UTF8.GetString(body);

            var deserializedMessage = JsonSerializer.Deserialize<TMessage>(message);
            if (deserializedMessage is null) return;

            handler.OnMessageReceivedAsync(deserializedMessage);
        };

        channel.BasicConsume(queue: handler.QueueName, consumer: consumer, autoAck: true);

        return Task.CompletedTask;
    }
}