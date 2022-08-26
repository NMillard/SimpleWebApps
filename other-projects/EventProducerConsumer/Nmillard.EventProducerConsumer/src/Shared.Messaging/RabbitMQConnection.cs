using RabbitMQ.Client;

namespace Shared.Messaging;

internal class RabbitMqConnection
{
    private readonly string hostName;
    private readonly string? clientName;
    private IConnection? connection;

    public RabbitMqConnection(string hostName, string? clientName = default)
    {
        if (string.IsNullOrEmpty(hostName)) throw new ArgumentNullException(nameof(hostName));
        this.hostName = hostName;
        this.clientName = clientName;
    }

    public IConnection Connection
    {
        get
        {
            if (connection is not null) return connection;
            var factory = new ConnectionFactory
            {
                HostName = hostName,
                Port = 5672,
                ClientProvidedName = clientName
            };

            connection = factory.CreateConnection();
            return connection;
        }
    }
}