using System;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;

namespace BeginnerVsSenior.Junior.WebAPI.Controllers {
    
    [Route("api/[controller]")]
    public class UsersController : ControllerBase {
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

        [HttpPost]
        public async Task<IActionResult> CreateUser(User user) {
            await user.SaveAsync(context);

            var client = new ServiceBusClient(serviceBusConnectionString);
            ServiceBusSender sender = client.CreateSender(topic);

            var message = new ServiceBusMessage(JsonSerializer.Serialize(new { UserId = user.Id }));
            await sender.SendMessageAsync(message);

            return Created($"api/users/{user.Id}/details", new { UserId = user.Id });
        }
    }
}