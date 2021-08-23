using System;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace BeginnerVsSenior.MidLevel {
    public class CreateUserHandler {
        private readonly IUserRepository repository;
        private readonly IMessagePublisher publisher;

        public CreateUserHandler(IUserRepository repository, IMessagePublisher publisher) {
            this.repository = repository;
            this.publisher = publisher;
        }

        public async Task<bool> ExecuteAsync(string username) {
            if (string.IsNullOrEmpty(username)) throw new ArgumentNullException(nameof(username));
            if (username.Length < 3) throw new ArgumentException("Cannot be less than 3 characters");
            if (username.Length > 50) throw new ArgumentException("Cannot exceed 50 characters");
            
            var user = new User(username);
            
            await repository.SaveAsync(user);
            await publisher.PublishAsync(new { UserId = user.Id });

            return true;
        }
    }

    public interface IUserRepository {
        Task<bool> SaveAsync(User user);
    }
    
    internal class SqlUserRepository : IUserRepository {
        private readonly AppDbContext context;

        public SqlUserRepository(AppDbContext context) => this.context = context;

        public async Task<bool> SaveAsync(User user) {
            await context.AddAsync(user);
            await context.SaveChangesAsync();
            
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