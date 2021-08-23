using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace BeginnerVsSenior.MidLevel {
    public class CreateUserHandler {
        private readonly AppDbContext context;
        private readonly IMessagePublisher publisher;

        public CreateUserHandler(AppDbContext context, IMessagePublisher publisher) {
            this.context = context;
            this.publisher = publisher;
        }

        public async Task<bool> ExecuteAsync(string email) {
            var user = new User(email);
            await context.AddAsync(user);
            await context.SaveChangesAsync();

            await publisher.PublishAsync(new { UserId = user.Id });

            return true;
        }
    }

    public interface IMessagePublisher {
        Task PublishAsync(object serializableMessage);
    }

    internal class AzureServiceBus : IMessagePublisher {
        private readonly ServiceBusSender topic;

        public AzureServiceBus(AzureBusOptions options) {
            var client = new ServiceBusClient(options.ServiceBusConnectionString);
            topic = client.CreateSender(options.Topic);
        }

        public async Task PublishAsync(object serializableMessage) {
            string json = JsonSerializer.Serialize(serializableMessage);
            await topic.SendMessageAsync(new ServiceBusMessage(json));
        }
    }

    internal class AzureBusOptions {
        public string ServiceBusConnectionString { get; set; }
        public string Topic { get; set; }
    }
}