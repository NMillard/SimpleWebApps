using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace Shared.Messaging;

internal class RabbitMqMessageProducer : IMessageProducer
{
    private readonly RabbitMqConnection connection;

    public RabbitMqMessageProducer(RabbitMqConnection connection)
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