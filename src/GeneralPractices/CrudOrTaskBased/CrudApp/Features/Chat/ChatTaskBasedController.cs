using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CrudApp.Domain;
using Microsoft.AspNetCore.Mvc;

namespace CrudApp.Features.Chat {
    
    [Route("api/chat")]
    public class ChatTaskBasedController : ControllerBase {
        private readonly ChatRepository repository;

        public ChatTaskBasedController(ChatRepository repository) => this.repository = repository;

        [HttpGet("Channel/{id:guid}/ViewDetails")]
        public async Task<IActionResult> ViewDetails(Guid id) {
            ChatChannel? channel = await repository.GetAsync(id);

            return channel is { }
                ? Ok(new { channel.Id, channel.Name })
                : NotFound();
        }
        
        [HttpGet("Channel/{id:guid}/ViewHistory")]
        public async Task<IActionResult> ViewHistory(Guid id) {
            ChatChannel? chatChannel = await repository.GetAsync(id);
            IReadOnlyList<ChatMessage> messages = chatChannel?.ChatMessages ?? new List<ChatMessage>();
            
            return Ok(messages);
        }

        [HttpPost("StartNew")]
        public async Task<IActionResult> StartNewChannel(StartNewChannel model) {
            var chatChannel = new ChatChannel(Guid.NewGuid(), model.Name);
            bool savedSuccessfully = await repository.AddAsync(chatChannel);

            return savedSuccessfully
                ? Created($"api/channel/{chatChannel.Id.ToString()}", chatChannel.Id)
                : BadRequest("Some error");
        }

        [HttpPatch("Channel/{id:guid}/ChangeName")]
        public IActionResult ChangeName(Guid id) {
            return Ok();
        }

        [HttpPost("Channel/{id:guid}/PublishMessage")]
        public async Task<IActionResult> PublishMessage(Guid id, PublishMessageModel model) {
            if (await repository.GetAsync(id) is not { } channel) return NotFound("Channel not found");
            
            var message = new ChatMessage(model.Content);
            channel.PublishMessage(message);

            bool savedSuccessfully = await repository.UpdateAsync(channel);

            return savedSuccessfully ? Ok() : BadRequest();
        }
        
        
        [HttpPost("Message/{id:guid}/reply")]
        public IActionResult Reply(Guid id) {
            return Ok();
        }
        
        [HttpGet("Message/{id:guid}/ShowReplies")]
        public IActionResult ShowReplies(Guid id) {
            return Ok();
        }

    }

    public record PublishMessageModel {
        public string Content { get; set; }
    }

    public record StartNewChannel {
        public string Name { get; set; }
    }
}