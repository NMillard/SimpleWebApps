using Microsoft.AspNetCore.Mvc;
using Shared.Messaging;

namespace Producer.WebAPi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly IMessageProducer producer;

    public MessagesController(IMessageProducer producer)
    {
        ArgumentNullException.ThrowIfNull(producer);
        this.producer = producer;
    }

    [HttpPost("CreateMessage")]
    public async Task<IActionResult> CreateMessage()
    {
        await producer.SendAsync(new Message
        {
            Payload = new {CorrelationId = Guid.NewGuid()},
            Topic = "message.created",
            ExchangeName = "messages",
        });

        return Ok();
    }
}