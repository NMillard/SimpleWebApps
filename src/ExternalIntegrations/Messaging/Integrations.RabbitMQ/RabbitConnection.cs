using RabbitMQ.Client;

namespace Integrations.RabbitMQ; 

public class RabbitConnection {
    private readonly string hostName;
    private IConnection? connection;
    
    public RabbitConnection(string hostName) {
        this.hostName = hostName;
    }

    public IConnection Connection {
        get {
            if (this.connection is not null) return connection;
            
            var factory = new ConnectionFactory { HostName = hostName, Port = 5672 };
            this.connection = factory.CreateConnection();

            return connection;
        }
    }
}