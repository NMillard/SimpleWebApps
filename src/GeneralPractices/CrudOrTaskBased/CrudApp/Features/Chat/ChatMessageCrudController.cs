using System;
using System.Threading.Tasks;
using CrudApp.Domain;
using Microsoft.AspNetCore.Mvc;

namespace CrudApp.Features.Chat {
    [Route("api/messages")]
    public class ChatMessageCrudController : ControllerBase {
        private readonly ChatRepository repository;

        public ChatMessageCrudController(ChatRepository repository) => this.repository = repository;

        [HttpPost]
        public async Task<IActionResult> Post(PostMessageRequest request) {
            ChatChannel? channel = await repository.GetAsync(request.ChannelId);
            if (channel is null) return BadRequest();
            
            channel.PublishMessage(new ChatMessage(request.Message));

            await repository.UpdateAsync(channel);
            
            return Ok();
        }

        [HttpDelete("{id:guid}")]
        public IActionResult Delete(Guid id) => Ok();
    }
}