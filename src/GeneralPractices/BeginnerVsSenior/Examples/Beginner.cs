using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace BeginnerVsSenior.Beginner {
    public class UsersController {
        private readonly AppDbContext context;
        private readonly string serviceBusConnectionString;
        private readonly string topic;

        public UsersController(AppDbContext context) {
            this.context = context;

            // Parameters required by the class should ideally be injected into the class, rather than
            // pulled from the environment.
            serviceBusConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings:ServiceBus");
            topic = Environment.GetEnvironmentVariable("ServiceBus:Topic");

        }

        public async Task<bool> CreateUser(string email) {
            var user = new User(email);
            await context.AddAsync(user);
            await context.SaveChangesAsync();

            var client = new ServiceBusClient(serviceBusConnectionString);
            ServiceBusSender sender = client.CreateSender(topic);

            var message = new ServiceBusMessage();
            await sender.SendMessageAsync(message);
            
            return true;
        }
    }
}