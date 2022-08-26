using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Shared.Messaging;

public abstract class MessageHandler<TMessage>
{
    public string ExchangeName { get; set; }
    public string QueueName { get; set; }
    public string Topic { get; set; }

    public abstract Task OnMessageReceived(TMessage message);
} 

internal class MessageConsumer<THandler, TMessage> : BackgroundService
    where THandler : MessageHandler<TMessage>
{
    private readonly THandler handler;
    private readonly RabbitMQConnection connection;

    public MessageConsumer(THandler handler, RabbitMQConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);
        this.handler = handler;
        this.connection = connection;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using IModel channel = connection.Connection.CreateModel();
        channel.ExchangeDeclare(handler.ExchangeName, ExchangeType.Topic, durable: true);
        channel.QueueDeclare(handler.QueueName, durable: true, exclusive: false);
        channel.QueueBind(handler.QueueName, handler.ExchangeName, handler.Topic);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (sender, args) =>
        {
            byte[] body =  args.Body.ToArray();
            string message = Encoding.UTF8.GetString(body);

            var deserialized = JsonSerializer.Deserialize<TMessage>(message);
            if (deserialized is null) return;

            await handler.OnMessageReceived(deserialized);
        };

        channel.BasicConsume(queue: handler.QueueName, consumer: consumer, autoAck: true);
    }
}

public class Message
{
    public string ExchangeName { get; set; }
    public string Topic { get; set; }
    public object Payload { get; set; }
}

public interface IMessageProducer
{
    Task<bool> SendAsync(Message message);
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMessaging(this IServiceCollection services, string hostName)
    {
        services.AddSingleton(_ => new RabbitMQConnection(hostName));
        services.AddScoped<IMessageProducer, RabbitMQMessageProducer>();
        
        return services;
    }

    public static IServiceCollection AddMessageConsumer<THandler, TMessage>(this IServiceCollection services, string hostName)
        where THandler : MessageHandler<TMessage>
    {
        services.AddSingleton(_ => new RabbitMQConnection(hostName));
        services.AddHostedService<MessageConsumer<THandler, TMessage>>();
        
        return services;
    }
}

internal class RabbitMQMessageProducer : IMessageProducer
{
    private readonly RabbitMQConnection connection;

    public RabbitMQMessageProducer(RabbitMQConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);
        this.connection = connection;
    }
    
    public Task<bool> SendAsync(Message message)
    {
        using IModel channel = connection.Connection.CreateModel();
        channel.ExchangeDeclare(message.ExchangeName, ExchangeType.Topic, durable: true, autoDelete: false);

        byte[] messageBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message.Payload));
        
        channel.BasicPublish(message.ExchangeName, message.Topic, null, messageBytes);

        return Task.FromResult(true);
    }
}


internal class RabbitMQConnection
{
    private readonly string hostName;
    private IConnection? connection;

    public RabbitMQConnection(string hostName)
    {
        if (string.IsNullOrEmpty(hostName)) throw new ArgumentNullException(nameof(hostName));
        this.hostName = hostName;
    }

    public IConnection Connection
    {
        get
        {
            if (connection is not null) return connection;
            var factory = new ConnectionFactory { HostName = hostName };

            connection = factory.CreateConnection();
            return connection;
        }
    }
}