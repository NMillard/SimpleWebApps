using System.Text;
using RabbitMQ.Client;

namespace Integrations.RabbitMQ;

public class Worker : BackgroundService {
    private readonly RabbitConnection factory;
    private readonly ILogger<Worker> logger;
    private const string Exchange = "nicm-direct";
    private const string Queue = "nicm-queue";

    public Worker(RabbitConnection factory, ILogger<Worker> logger) {
        this.factory = factory;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        using IModel model = factory.Connection.CreateModel();
        
        model.ExchangeDeclare(Exchange, ExchangeType.Direct, durable: true);
        model.QueueDeclare(Queue, durable: true, exclusive: false);
        model.QueueBind(queue: Queue, exchange: Exchange, routingKey: "app");
        
        while (!stoppingToken.IsCancellationRequested) {
            model.BasicPublish(Exchange, routingKey: "app", body: Encoding.UTF8.GetBytes("hello"));

            logger.LogInformation("Message sent");
            await Task.Delay(10000, stoppingToken);
        }
    }
}