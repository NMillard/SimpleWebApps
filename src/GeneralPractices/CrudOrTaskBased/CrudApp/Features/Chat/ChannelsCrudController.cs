using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CrudApp.Domain;
using Microsoft.AspNetCore.Mvc;

namespace CrudApp.Features.Chat {
    
    [ApiController]
    [Route("api/channels")]
    public class ChannelsCrudController : ControllerBase {
        private readonly ChatRepository repository;

        public ChannelsCrudController(ChatRepository repository) => this.repository = repository;

        [HttpGet]
        public async Task<IActionResult> Get() {
            IEnumerable<ChatChannel> channels = await repository.GetAsync();
            
            return Ok(new {
                Channels = channels.Select(c => new { c.Id, c.Name }),
                Links = new[] {
                    new Hateoas { Href = "api/channels/{{id}}", Rel = "Channels", Type = "GET" }
                }
            });
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id) {
            ChatChannel? channel = await repository.GetAsync(id);
            if (channel is null) return BadRequest();
            
            return Ok(new {
                channel,
                Links = new[] {
                    new Hateoas { Href = "api/channels", Rel = "Channels", Type = "GET" },
                    new Hateoas { Href = "api/channels", Rel = "Channels", Type = "POST" },
                    new Hateoas { Href = $"api/channels/{id}/messages", Rel = "ChatMessages", Type = "GET" },
                    new Hateoas { Href = "api/messages", Rel = "ChatMessages", Type = "POST" },
                }
            });
        }

        [HttpPost]
        public async Task<IActionResult> Post(CreateChannelRequest request) {
            var channelId = Guid.NewGuid();
            await repository.AddAsync(new ChatChannel(channelId, request.ChannelName));
            
            return Created($"api/channels/{channelId}", channelId);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id) {
            if (await repository.GetAsync(id) is not { } channel) return BadRequest();
            bool deleted = await repository.DeleteAsync(channel);
            
            return deleted ? NoContent() : BadRequest();
        }

        [HttpPatch("{id:guid}")]
        public IActionResult Patch(Guid id) => Ok();

        [HttpPost("{id:guid}/messages")]
        public async Task<IActionResult> PostMessage(Guid id, PostMessageRequest request) {
            ChatChannel? channel = await repository.GetAsync(id);
            if (channel is null) return BadRequest();

            var chatMessage = new ChatMessage(request.Message);
            channel.PublishMessage(chatMessage);

            await repository.UpdateAsync(channel);
            
            return Created($"api/channels/{id}/messages", chatMessage);
        }

        [HttpGet("{id:guid}/messages")]
        public async Task<IActionResult> GetMessages(Guid id) {
            ChatChannel? channel = await repository.GetAsync(id);
            if (channel is null) return BadRequest();

            return Ok(channel.ChatMessages);
        }
    }

    public class CreateChannelRequest {
        [Required]
        [MaxLength(100)]
        public string ChannelName { get; set; }
    }
}