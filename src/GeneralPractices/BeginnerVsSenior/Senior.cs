using System;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace BeginnerVsSenior.Senior {
    public class CreateUserHandler {
        private readonly AppDbContext context;
        private readonly AzureBusOptions options;

        public CreateUserHandler(AppDbContext context, AzureBusOptions options) {
            this.context = context;
            this.options = options;
        }

        public async Task<bool> ExecuteAsync(string email) {
            if (string.IsNullOrEmpty(email)) throw new ArgumentNullException(nameof(email));
            // other relevant checks
            
            var user = new User(email);
            await SaveUser(user);
            await PublishMessage(user.Id);
            
            return true;
        }

        private async Task SaveUser(User user) {
            await context.AddAsync(user);
            await context.SaveChangesAsync();
        }

        private async Task PublishMessage(Guid userId) {
            var client = new ServiceBusClient(options.ServiceBusConnectionString);
            ServiceBusSender topic = client.CreateSender(options.Topic);
            
            string json = JsonSerializer.Serialize(new { UserId = userId });
            await topic.SendMessageAsync(new ServiceBusMessage(json));
        }
    }
    
    public class AzureBusOptions {
        public string ServiceBusConnectionString { get; set; }
        public string Topic { get; set; }
    }
}