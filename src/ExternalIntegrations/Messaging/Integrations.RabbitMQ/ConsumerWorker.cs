using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Integrations.RabbitMQ; 

public class Consumer : BackgroundService {
    private readonly RabbitConnection factory;
    private readonly ILogger<Consumer> logger;

    public Consumer(RabbitConnection factory, ILogger<Consumer> logger) {
        this.factory = factory;
        this.logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        using IModel model = factory.Connection.CreateModel();

        model.QueueDeclare("nicm-queue", durable: true, exclusive: false);
        model.QueueBind("nicm-queue", "nicm-direct", routingKey: "app");
        
        var consumer = new EventingBasicConsumer(model);
        consumer.Received += (_, args) => {
            string message = Encoding.UTF8.GetString(args.Body.ToArray());
            logger.LogInformation("Message received: {Message}", message);
        };

        model.BasicConsume("nicm-queue", autoAck: true, consumer);
        
        while (!stoppingToken.IsCancellationRequested) { }
    }
}